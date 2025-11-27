using System.Collections.Generic;
using NUnit.Framework;

namespace Systems.Spawning.Runtime
{
    public class Spawner
    {
        public List<SpawnEvent> Advance(float f)
        {
            throw new System.NotImplementedException();
        }

        public float SpawnRate { get; set; }
        public float Randomness { get; set; }
    }
}