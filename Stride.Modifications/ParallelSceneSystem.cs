using Stride.Core;
using Stride.Engine;
using Stride.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stride.Modifications
{
    public class ParallelSceneSystem : SceneSystem
    {
        List<SceneInstance> instances = new List<SceneInstance>();

        public ParallelSceneSystem(IServiceRegistry registry) : base(registry)
        {
        }

        public void Add(SceneInstance sceneInstance)
        {
            instances.Add(sceneInstance);
        }

        public void Remove(SceneInstance sceneInstance)
        {
            instances.Remove(sceneInstance);
        }

        public override void Update(GameTime gameTime)
        {
            // SceneSystem.Update only calls SceneInstance property's update method
            // so we WONT call it here, to avoid duplicating calls
            //base.Update(gameTime);

            foreach (var instance in instances)
                instance.Update(gameTime);
        }

        public void ChangePrimaryScene()
        {

        }
    }
}
