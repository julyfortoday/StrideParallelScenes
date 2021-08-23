using Stride.Core.Mathematics;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.Rendering;
using Stride.Rendering.Lights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ParallelScenes
{
    public class SceneGenerator
    {
        Random rng;
        ContentManager Content;
        public SceneGenerator(ContentManager c)
        {
            rng = new Random(666);
            Content = c;
            InitAssets();
        }

        List<Model> Models = new List<Model>();
        List<Material> Materials = new List<Material>();

        private void InitAssets()
        {
            Models.Add(Content.Load<Model>("Cone"));
            Models.Add(Content.Load<Model>("Sphere"));
            Models.Add(Content.Load<Model>("Box"));
            Models.Add(Content.Load<Model>("Teapot"));

            Materials.Add(Content.Load<Material>("Sphere Material"));
        }

        public Scene LoadScene(string name)
        {
            Scene s = Content.Load<Scene>("Scenes/" + name);
            s.Name = name;
            return s;
        }

        public Scene MakeScene(string name)
        {
            var s = new Scene();
            s.Name = name;
            CreateEnvironment(s);
            CreateObjects(s);
            return s;
        }

        public List<Color> GetAllStrideColors()
        {
            return typeof(Color)
                      .GetFields(BindingFlags.Public | BindingFlags.Static)
                      .Where(f => f.FieldType == typeof(Color))
                      .Select(f => (Color) f.GetValue(null)).ToList();
        }

        private void CreateEnvironment(Scene s)
        {
            var ground = CreateEntity(
                Content.Load<Model>("Ground"),
                GetRandomFromList(Materials));
            s.Entities.Add(ground);

            var sun = new Entity();
            sun.Transform.Rotation = Quaternion.RotationYawPitchRoll(-30,180,0);
            var lc = new LightComponent();
            var dirLight = new LightDirectional();
            dirLight.Shadow.Enabled = true;
            lc.Type = dirLight;
            lc.Intensity = 20f;
            lc.Enabled = true;
            var color = GetRandomFromList(GetAllStrideColors()).ToColor3();
            lc.SetColor(color);
            sun.Add(lc);

            s.Entities.Add(sun);
        }

        private void CreateObjects(Scene s)
        {
            var objectCount = rng.Next(1,20);

            foreach (int i in Enumerable.Range(1, objectCount))
            {
                var entity = CreateEntity(GetRandomFromList(Models), GetRandomFromList(Materials));
                entity.Transform.Position.X = (i * 2);
                entity.Transform.Position.Y = 0.5f;
                s.Entities.Add(entity);
            }
        }

        private Entity CreateEntity(Model model, Material mat)
        {
            Entity e = new Entity();
            var mc = new ModelComponent();
            mc.Model = model;
            e.Add(mc);
            mc.Materials[0] = mat;
            return e;
        }

        private T GetRandomFromList<T>(List<T> list)
        {
            return list[rng.Next(0, list.Count)];
        }
    }
}
