namespace Systems.Spawning.Runtime
{
    public class SpawnTimer
    {
        private float timeSinceLastSpawn;
        private float interval = float.MaxValue;

        private float spawnRate;
        public float SpawnRate
        {
            get => spawnRate;
            set
            {
                spawnRate = value;

                if (spawnRate > 0f)
                    interval = 1f / spawnRate;
                else
                    interval = float.PositiveInfinity;
            }
        }

        public int Advance(float deltaTime)
        {
            if (SpawnRate <= 0f)
            {
                return 0;
            }

            timeSinceLastSpawn += deltaTime;

            int spawns = 0;
            while (timeSinceLastSpawn >= interval)
            {
                timeSinceLastSpawn -= interval;
                spawns += 1;
            }

            return spawns;
        }
    }
}