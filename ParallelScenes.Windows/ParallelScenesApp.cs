using Stride.Engine;

namespace ParallelScenes
{
    class ParallelScenesApp
    {
        static void Main(string[] args)
        {
            using (var game = new MyGame())
            {
                game.Run();
            }
        }
    }
}
