using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Core.Diagnostics;

namespace ParallelScenes
{
    public class Rotator : SyncScript
    {
        //public override void Start()
        //{
        //}

        double time;
        const float speed = 0.01f;
        public override void Update()
        {
            time += Game.UpdateTime.Elapsed.TotalMilliseconds;

            if (time > 2000)
            {
                time = 0;
                Log.Debug("[ scene update "+ this.Entity.Scene.Name + "]");
            }
            RotateTransform(new Vector3(speed,0,0));
        }

        public void RotateTransform(Vector3 vector)
        {
            Entity.Transform.Rotation *= Quaternion.RotationYawPitchRoll(
              vector.X,
              vector.Y,
              vector.Z);
        }
    }
}
