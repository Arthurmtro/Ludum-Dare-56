using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Germinator
{
    public readonly struct EnemyTypeQuantity
    {
        public EnemyTypeQuantity(EnemyType type, int quantity)
        {
            Type = type;
            Quantity = quantity;
        }

        public EnemyType Type { get; }
        public int Quantity { get; }
    }

    [Serializable]
    public struct EnemyWaveStep
    {
        public EnemyWaveStep(float time, EnemyType type, int quantity)
        {
            Time = time;
            Type = type;
            Quantity = quantity;
        }

        // Time when the step should be triggered
        public float Time;
        // Type of enemy to spawn
        public EnemyType Type;
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

        public readonly EnemyTypeQuantity[] GetQuantities()
        {
            List<EnemyTypeQuantity> result = new();
            foreach (var step in Steps)
            {
                if (step.Quantity > 0)
                {
                    result.Add(new EnemyTypeQuantity(step.Type, step.Quantity));
                }
            }

            return result.ToArray();
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

        public EnemyWaveBuilder AddStep(float time, EnemyType type, int quantity)
        {
            steps.Add(new EnemyWaveStep(time, type, quantity));
            return this;
        }

        public EnemyWave Build()
        {
            return new EnemyWave(duration, steps.ToArray());
        }

        public static EnemyWave Random(float duration, int steps, int minEnemies, int maxEnemies, EnemyType[] types)
        {
            EnemyWaveBuilder builder = new EnemyWaveBuilder().SetDuration(duration);
            System.Random random = new System.Random();
            float maxTime = 0.8f * duration;
            float stepDuration = maxTime / steps;
            for (int i = 0; i < steps; i++)
            {
                float time = Math.Max(0.1f, (float)(i * stepDuration));
                EnemyType type = types[random.Next(0, types.Length - 1)];
                int quantity = (int)Math.Round(Mathf.Lerp(minEnemies, maxEnemies, (float)i / steps));

                builder.AddStep(time, type, quantity);
            }

            return builder.Build();
        }

        internal static void Random(float duration, float v1, double v2, float v3, EnemyType[] enemyTypes)
        {
            throw new NotImplementedException();
        }
    }
}