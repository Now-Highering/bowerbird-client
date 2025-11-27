using System.Collections.Generic;
using NUnit.Framework;
using Systems.Spawning.Runtime;

namespace Systems.Spawning.Tests
{
    public class SpawnerTests
    {
        private Spawner s;
        
        [SetUp]
        public void Setup()
        {
            s = new Spawner();
        }

        [TearDown]
        public void TearDown()
        {
            s = null;
        }
        
        [TestCase(0f,1000f, 0)]
        [TestCase(1f,1f, 1)]
        [TestCase(1f, 100f, 100)]
        [TestCase(.01f, 100f, 1)]
        [TestCase(.5f, 10f, 5)]
        public void Spawner_SpawnRate(
            float spawnRate,
            float durationSeconds,
            int expectedCount)
        {
            s.SpawnRate = spawnRate;

            float frame = 1f / 60f;
            var spawns = SimulateForDuration(durationSeconds, frame);

            Assert.AreEqual(expectedCount, spawns.Count);
        }

        private List<SpawnEvent> SimulateForDuration(float durationSeconds, float frameDeltaSeconds)
        {
            var events = new List<SpawnEvent>();
            float elapsed = 0f;

            while (elapsed < durationSeconds)
            {
                elapsed += frameDeltaSeconds;
                events.AddRange(s.Advance(frameDeltaSeconds));
            }
            
            return events;
        }
    }
}