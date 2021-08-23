using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Core.Diagnostics;
using Stride.Physics;

namespace ParallelScenes
{
    public class PhysicsRotator : SyncScript
    {
        RigidbodyComponent rc;

        public override void Start()
        {
            rc = Entity.Get<RigidbodyComponent>();
        }

        double time;
        const float speed = 0.005f;
        public override void Update()
        {
            time += Game.UpdateTime.Elapsed.TotalMilliseconds;

            if (time > 2000)
            {
                time = 0;
                Log.Debug("[ scene update " + this.Entity.Scene.Name + "]");
            }

            var direction = new Vector3(0, 1, 0);
            direction.Normalize();
            rc.ApplyTorque(direction * speed);
        }
    }
}
