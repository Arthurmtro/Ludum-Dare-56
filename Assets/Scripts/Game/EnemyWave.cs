using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Germinator
{
    public struct EnemyBuilderQuantity
    {
        public EnemyBuilderQuantity(EnemyController prefab, int quantity)
        {
            Prefab = prefab;
            Quantity = quantity;
        }

        public EnemyController Prefab { get; }
        public int Quantity;
        public void SetQuantity(int newValue)
        {
            Quantity = newValue;
        }
        public void AddQuantity(int newValue)
        {
            Quantity += newValue;
        }
    }

    [Serializable]
    public struct EnemyWaveStep
    {
        public EnemyWaveStep(float time, EnemyController prefab, int quantity)
        {
            Time = time;
            Prefab = prefab;
            Quantity = quantity;
        }

        // Time when the step should be triggered
        public float Time;
        // Type of enemy to spawn
        public EnemyController Prefab;
        // Quantity of enemies to spawn
        public int Quantity;
    }

    [Serializable]
    public struct EnemyWave
    {
        public EnemyWave(float duration, EnemyWaveStep[] steps)
        {
            Duration = duration;
            Steps = steps;
        }

        // Duration in seconds of the wave
        public float Duration;
        // Steps of the wave
        public EnemyWaveStep[] Steps;

        public readonly EnemyBuilderQuantity[] GetQuantities()
        {
            Dictionary<int, EnemyBuilderQuantity> result = new();
            foreach (var step in Steps)
            {
                if (step.Quantity > 0)
                {
                    if (result.ContainsKey(step.Prefab.key))
                    {
                        int current = result[step.Prefab.key].Quantity;
                        result.Remove(step.Prefab.key);
                        result.Add(step.Prefab.key, new EnemyBuilderQuantity(step.Prefab, current + step.Quantity));
                    }
                    else
                    {
                        result.Add(step.Prefab.key, new EnemyBuilderQuantity(step.Prefab, step.Quantity));
                    }
                }
            }

            return result.Values.ToArray();
        }
    }

    public class EnemyWaveBuilder
    {
        private float duration = 60;
        private List<EnemyWaveStep> steps = new();

        public EnemyWaveBuilder SetDuration(float duration)
        {
            this.duration = duration;
            return this;
        }

        public EnemyWaveBuilder AddStep(float time, EnemyController prefab, int quantity)
        {
            steps.Add(new EnemyWaveStep(time, prefab, quantity));
            return this;
        }

        public EnemyWave Build()
        {
            return new EnemyWave(duration, steps.ToArray());
        }

        public static EnemyWave Random(float duration, int steps, int minEnemies, int maxEnemies, EnemyController[] prefabs)
        {
            EnemyWaveBuilder waveBuilder = new EnemyWaveBuilder().SetDuration(duration);
            System.Random random = new System.Random();
            float maxTime = 0.8f * duration;
            float stepDuration = maxTime / steps;
            for (int i = 0; i < steps; i++)
            {
                float time = Math.Max(2f, (float)(i * stepDuration));
                EnemyController prefab = prefabs[random.Next(0, prefabs.Length - 1)];
                int quantity = (int)Math.Round(Mathf.Lerp(minEnemies, maxEnemies, (float)i / steps));

                Debug.Log($"{time}: {quantity}");
                waveBuilder.AddStep(time, prefab, quantity);
            }

            return waveBuilder.Build();
        }

        internal static void Random(float duration, float v1, double v2, float v3, EnemyController[] prefabs)
        {
            throw new NotImplementedException();
        }
    }
}