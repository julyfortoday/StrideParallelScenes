using Stride.Core;
using Stride.Core.Diagnostics;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Games;
using Stride.Modifications;
using Stride.Rendering;
using Stride.Rendering.Compositing;
using Stride.Rendering.Lights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Stride.Engine.Scene;

namespace ParallelScenes
{
    public class MyGame : Game
    {
        EventReceiver NextSceneListener = new EventReceiver(HUD.NextSceneEventKey);
        EventReceiver PreviousSceneListener = new EventReceiver(HUD.PreviousSceneEventKey);

        public MyGame()
        {
            Log.Info("Starting...");

            // preferrably MyGame would handle managing the multiple scenes, ParallelSceneSystem would
            // but since I can't modify Game's property to use it
            // I'm basically putting all the logic I would have put in that class over here
            
            //SceneSystem = new ParallelSceneSystem(this.Services);
        }

        List<SceneInstance> instances = new List<SceneInstance>();

        protected override Task LoadContent()
        {
            SceneGenerator gen = new SceneGenerator(Content);
            SceneSystem.SceneInstance.Name = "main";
            instances.Add(new SceneInstance(this.Services, gen.LoadScene("Scene01")) { Name = "Scene01" });
            instances.Add(new SceneInstance(this.Services, gen.LoadScene("Scene02")) { Name = "Scene02" });

            const int MAX_SCENE_COUNT = 40;

            var rng = new Random(315);
            var amount = rng.Next(1, MAX_SCENE_COUNT);

            foreach (int i in Enumerable.Range(1, amount))
            {
                string name = "BuiltScene" + i;
                instances.Add(new SceneInstance(this.Services, gen.MakeScene(name)) { Name = name });
            }
            AttachScene(0);
            return base.LoadContent();
        }
        protected override void Update(GameTime gameTime)
        {
            try
            {
                base.Update(gameTime);

                // manually update instances not loaded to the SceneSystem
                foreach (var instance in instances)
                {
                    if (instance == SceneSystem.SceneInstance)
                        continue;
                    if (SceneSystem.SceneInstance.RootScene.Children.Contains(instance.RootScene))
                        continue;

                    instance.Update(gameTime);
                }

                if (NextSceneListener.TryReceive())
                    LoadNextScene();

                if (PreviousSceneListener.TryReceive())
                    LoadPreviousScene();
            }
            catch(Exception e)
            {
                Log.Error(e.Message);
            }
        }

        private int CycleNumberUp(int index, int limit)
        {
            int next = index + 1;
            if (next >= limit)
                next = 0;
            return next;
        }
        private int CycleNumberDown(int index, int limit)
        {
            int next = index - 1;
            if (next < 0)
                next = (limit -1);
            return next;
        }

        int sceneIndex = 0;
        private void LoadNextScene()
        {
            sceneIndex = CycleNumberUp(sceneIndex, instances.Count);
            AttachScene(sceneIndex);
        }

        private void LoadPreviousScene()
        {
            sceneIndex = CycleNumberDown(sceneIndex, instances.Count);
            AttachScene(sceneIndex);
        }


        private void AttachScene(int sceneIndex)
        {
            try
            {
                var instance = instances[sceneIndex];
                Log.Info("going to scene: " + instance.RootScene?.Name);

                // recapture the current scene to prepare for loading a different scene
                var currentScene = SceneSystem.SceneInstance.RootScene.Children.FirstOrDefault();
                if (currentScene != null)
                {
                    SceneCollection sc = SceneSystem.SceneInstance.RootScene.Children as SceneCollection;
                    sc.Remove(currentScene);
                    SceneInstance originalInstance = instances.Where(x => x.Name == currentScene.Name).FirstOrDefault();
                    originalInstance.RootScene = currentScene;
                }


                // get the desired scene and put it in place
                var scene = instance.RootScene;
                instance.RootScene = null;
                scene.Parent = null; // trying to null set a scene's parent seemingly doesn't work -- Do i still need/want this?
                SceneSystem.SceneInstance.RootScene.Children.Add(scene);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }


        // some experimental code for trying to manage the camera/compositors while switching between scenes as well
        //private void ChangeInstance(int sceneIndex)
        //{
        //    WriteLog("scene: " + sceneIndex);
        //    var instance = instances[sceneIndex];

        //    var rc = RenderContext.GetShared(Services);
        //    var gc = rc.Tags.Get(GraphicsCompositor.Current);
        //    if(gc != null)
        //    {
        //        for (int i = 0; i < gc.Cameras.Count; i++)
        //            gc.Cameras[i] = null;
        //    }

        //    SceneSystem.SceneInstance = instance;
        //}
    }
}
