using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Systems.Spawning.Runtime
{
    public class SpawnTable
    {
        private float totalChance = 0f;
        private Dictionary<string, float> weights = new();
        
        public Dictionary<string, float> GetWeights => new(weights);
        
        public string Roll()
        {
            if (totalChance <= 0f)
            {
                throw new System.Exception("Weights has no entries with a chance to be rolled.");
            }
            
            float result = Random.Range(0f, totalChance);
            float total = 0;
            foreach (var key in weights.Keys)
            {
                total += weights[key];

                if (result <= total)
                {
                    return key;
                }
            }

            throw new Exception("Rolled higher than any weights");
        }

        private void Recalculate()
        {
            totalChance = 0f;
            foreach (var key in weights.Keys)
            {
                totalChance += weights[key];
            }
        }
        
        public void SetOrUpdateWeights(Dictionary<string, float> weights)
        {
            if (weights.Keys.Count == 0)
            {
                throw new System.ArgumentException("At least one weight must be defined.");
            }
            
            foreach (var key in weights.Keys)
            {
                SetOrUpdateWeightsInternal(key, weights[key]);
            }
            Recalculate();
        }

        public double GetChance(string id)
        {
            if (!weights.ContainsKey(id))
            {
                throw new KeyNotFoundException("Weights has no entry for id " + id);
            }

            return weights[id];
        }

        public void SetOrUpdateWeight(string id, float chance)
        {
            SetOrUpdateWeightsInternal(id, chance);
            Recalculate();
        }

        private void SetOrUpdateWeightsInternal(string id, float chance)
        {
            if (chance < 0)
            {
                throw new System.ArgumentException("Chance must be greater or equal to 0f");
            }
            weights[id] = chance;
        }

        public void ClearWeights()
        {
            weights.Clear();
        }
    }
}