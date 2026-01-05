using System.Collections.Generic;
using NUnit.Framework;
using Systems.Spawning.Runtime;

namespace Systems.Spawning.Tests
{
    public class SpawnTimerTests
    {
        private readonly float frameDuration = 1f/60;
        private SpawnTimer spawnTimer;
        
        [SetUp]
        public void Setup()
        {
            spawnTimer = new SpawnTimer();
        }

        [TearDown]
        public void TearDown()
        {
            spawnTimer = null;
        }
        
        [TestCase(0f,1000f, 0)]
        [TestCase(100f,0f, 0)]
        [TestCase(1f,1f, 1)]
        [TestCase(1f, 100f, 100)]
        [TestCase(.01f, 100f, 1)]
        [TestCase(.5f, 10f, 5)]
        public void SpawnRate(
            float spawnRate,
            float durationSeconds,
            int expectedCount)
        {
            // add .1f to make up for rounding errors due to float calculation
            if (durationSeconds > 1f)
            {
                durationSeconds += .1f;
            }
            
            spawnTimer.SpawnRate = spawnRate;

            int spawns = SimulateForDuration(durationSeconds);

            Assert.AreEqual(expectedCount, spawns);
        }
        
        // helper methods
        private int SimulateForDuration(float durationSeconds)
        {
            float elapsed = 0f;
            int spawns = 0;

            while (elapsed < durationSeconds)
            {
                elapsed += frameDuration;
                spawns += spawnTimer.Advance(frameDuration);
            }

            return spawns;
        }
    }
}