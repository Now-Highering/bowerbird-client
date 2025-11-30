using System;
using System.Collections.Generic;
using NUnit.Framework;
using Systems.Spawning.Runtime;

namespace Systems.Spawning.Tests
{
    public class SpawnTableTests
    {
        private SpawnTable spawnTable;
        
        [SetUp]
        public void Setup()
        {
            spawnTable = new SpawnTable();
        }

        [TearDown]
        public void TearDown()
        {
            spawnTable = null;
        }
        
        [Test]
        public void SetOrUpdateWeights_Setup()
        {
            Assert.Zero(spawnTable.GetWeights.Keys.Count);
            
            spawnTable.SetOrUpdateWeights(new Dictionary<string, float>
            {
                { "a", 1f },
                { "b", 2f }
            });

            Assert.AreEqual(2, spawnTable.GetWeights.Keys.Count);

            Assert.AreEqual(1f, spawnTable.GetChance("a"));
            Assert.AreEqual(2f, spawnTable.GetChance("b"));
        }

        [Test]
        public void SetOrUpdateWeights_ReplacesEntries()
        {
            spawnTable.SetOrUpdateWeights(new Dictionary<string, float>
            {
                { "a", 1f },
                { "b", 2f }
            });

            spawnTable.SetOrUpdateWeights(new Dictionary<string, float>
            {
                { "a", 100f },
                { "b", 200f }
            });
            
            Assert.AreEqual(100f, spawnTable.GetChance("a"));
            Assert.AreEqual(200f, spawnTable.GetChance("b"));
        }
        
        [Test]
        public void SetOrUpdateWeights_IgnoresExtras()
        {
            Assert.Zero(spawnTable.GetWeights.Count);
            
            spawnTable.SetOrUpdateWeights(new Dictionary<string, float>
            {
                { "a", 1f },
                { "b", 2f }
            });

            spawnTable.SetOrUpdateWeights(new Dictionary<string, float>
            {
                { "a", 15f },
            });
            
            Assert.AreEqual(15f, spawnTable.GetChance("a"));
            Assert.AreEqual(2f, spawnTable.GetChance("b"));
        }
        
        [Test]
        public void SetWeight_AddsEntry()
        {
            Assert.Zero(spawnTable.GetWeights.Count);
            
            spawnTable.SetOrUpdateWeight("a", 2f);

            Assert.AreEqual(1, spawnTable.GetWeights.Count);

            spawnTable.SetOrUpdateWeight("b", 1f);

            Assert.AreEqual(2, spawnTable.GetWeights.Keys.Count);
            
            Assert.AreEqual(2f, spawnTable.GetChance("a"));
            Assert.AreEqual(1f, spawnTable.GetChance("b"));
        }
        
        [Test]
        public void SetWeight_ReplacesEntry()
        {
            spawnTable.SetOrUpdateWeight("a", 1f);
            spawnTable.SetOrUpdateWeight("a", 10f);

            Assert.AreEqual(1, spawnTable.GetWeights.Keys.Count);
            
            Assert.AreEqual(10f, spawnTable.GetChance("a"));
        }

        [Test]
        public void GetChance_ThrowsIfDoesntExist()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                spawnTable.GetChance("a");
            });
        }
        
        [Test]
        public void GetChance()
        {
            spawnTable.SetOrUpdateWeight("a", 1f);
            Assert.AreEqual(1f, spawnTable.GetChance("a"));
        }
        
        [Test]
        public void GetWeightsIsCopy()
        {
            spawnTable.SetOrUpdateWeight("a", 1f);
            
            Dictionary<string, float> weights = spawnTable.GetWeights;
            weights["a"] = 2f;
            
            Assert.AreEqual(1f, spawnTable.GetWeights["a"]);
        }
        
        [Test]
        public void Reset_ClearsWeights()
        {
            spawnTable.SetOrUpdateWeight("a", 2f);
            spawnTable.SetOrUpdateWeights(new Dictionary<string, float>
            {
                { "b", 4f },
                { "c", 8f }
            });

            spawnTable.ClearWeights();
            Assert.Zero(spawnTable.GetWeights.Count);
        }
        
        [Test]
        public void Roll_NoWeights()
        {
            Assert.Throws<Exception>(() =>
            {
                spawnTable.Roll();
            });
        }
        
        [Test]
        public void SingleEntryTable_AlwaysReturnsThatEntry()
        {
            spawnTable.SetOrUpdateWeights(new Dictionary<string, float>
            {
                { "a", 100f }
            });

            for (int i = 0; i < 100; i++)
                Assert.AreEqual("a", spawnTable.Roll());
        }

        [Test]
        public void ZeroWeightEntries_AreNeverSelected()
        {
            spawnTable.SetOrUpdateWeights(new Dictionary<string, float>
            {
                { "a", 0f },
                { "b", 1f }
            });

            var counts = RollMany(spawnTable, 1000);

            Assert.False(counts.ContainsKey("a"));
            Assert.AreEqual(1000, counts["b"]);
        }

        [Test]
        public void TwoEntryTable_ProducesRoughlyCorrectFrequencies()
        {
            spawnTable.SetOrUpdateWeights(new Dictionary<string, float>
            {
                { "a", 99f },
                { "b", 1f }
            });

            var counts = RollMany(spawnTable, 10000);

            float freqA = counts.ContainsKey("a") ? counts["a"] / 10000f : 0f;
            float freqB = counts.ContainsKey("b") ? counts["b"] / 10000f : 0f;

            // Wide tolerances
            Assert.That(freqA, Is.InRange(0.9f, 1.1f));
            Assert.That(freqB, Is.InRange(0f, 0.1f));
        }

        [Test]
        public void EmptyTable_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                spawnTable.SetOrUpdateWeights(new Dictionary<string, float>());
            });
        }

        [Test]
        public void NegativeWeight_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                spawnTable.SetOrUpdateWeights(new Dictionary<string, float>
                {
                    { "a", 0 - float.Epsilon }
                });
            });
            
            Assert.Throws<ArgumentException>(() =>
            {
                spawnTable.SetOrUpdateWeight("a", 0 - float.Epsilon);
            });
        }

        // helper methods
        private Dictionary<string, int> RollMany(SpawnTable table, int trials)
        {
            var counts = new Dictionary<string, int>();

            for (int i = 0; i < trials; i++)
            {
                string result = table.Roll();

                if (!counts.ContainsKey(result))
                    counts[result] = 0;

                counts[result]++;
            }

            return counts;
        }
    }
}
