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

        private Entity CameraEntity;
        private Entity HUDEntity;

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

            CameraEntity = SceneSystem.SceneInstance.RootScene.Entities.First(e => e.Name == "Camera");
            HUDEntity = SceneSystem.SceneInstance.RootScene.Entities.First(e => e.Name == "HUD");


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

                SceneSystem.SceneInstance.RootScene.Entities.Remove(HUDEntity);
                SceneSystem.SceneInstance.RootScene.Entities.Remove(CameraEntity);

                SceneSystem.SceneInstance = instance;

                SceneSystem.SceneInstance.RootScene.Entities.Add(CameraEntity);
                SceneSystem.SceneInstance.RootScene.Entities.Add(HUDEntity);

            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}
