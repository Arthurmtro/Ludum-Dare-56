using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Germinator
{
    public readonly struct EnemyBuilderQuantity
    {
        public EnemyBuilderQuantity(EnemyBuilder builder, int quantity)
        {
            Builder = builder;
            Quantity = quantity;
        }

        public EnemyBuilder Builder { get; }
        public int Quantity { get; }
    }

    [Serializable]
    public struct EnemyWaveStep
    {
        public EnemyWaveStep(float time, EnemyBuilder builder, int quantity)
        {
            Time = time;
            Builder = builder;
            Quantity = quantity;
        }

        // Time when the step should be triggered
        public float Time;
        // Type of enemy to spawn
        public EnemyBuilder Builder;
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
            List<EnemyBuilderQuantity> result = new();
            foreach (var step in Steps)
            {
                if (step.Quantity > 0)
                {
                    result.Add(new EnemyBuilderQuantity(step.Builder, step.Quantity));
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

        public EnemyWaveBuilder AddStep(float time, EnemyBuilder builder, int quantity)
        {
            steps.Add(new EnemyWaveStep(time, builder, quantity));
            return this;
        }

        public EnemyWave Build()
        {
            return new EnemyWave(duration, steps.ToArray());
        }

        public static EnemyWave Random(float duration, int steps, int minEnemies, int maxEnemies, EnemyBuilder[] builders)
        {
            EnemyWaveBuilder waveBuilder = new EnemyWaveBuilder().SetDuration(duration);
            System.Random random = new System.Random();
            float maxTime = 0.8f * duration;
            float stepDuration = maxTime / steps;
            for (int i = 0; i < steps; i++)
            {
                float time = Math.Max(0.1f, (float)(i * stepDuration));
                EnemyBuilder builder = builders[random.Next(0, builders.Length - 1)];
                int quantity = (int)Math.Round(Mathf.Lerp(minEnemies, maxEnemies, (float)i / steps));

                waveBuilder.AddStep(time, builder, quantity);
            }

            return waveBuilder.Build();
        }

        internal static void Random(float duration, float v1, double v2, float v3, EnemyBuilder[] enemyBuilders)
        {
            throw new NotImplementedException();
        }
    }
}