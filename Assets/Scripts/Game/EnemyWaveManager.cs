using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Germinator
{
    public class EnemyWaveManager : MonoBehaviour
    {
        #region Input

        [Header("Managers")]
        [SerializeField] private EnemyWaveCollection waveCollection;
        private EnemyManager enemyManager;

        // Number of ticks to avoid checking next steps
        [SerializeField][Range(10, 60)] private int sleepTicks = 10;

        [Header("Random waves")]
        [SerializeField][Range(30, 120)] private float duration;
        [SerializeField][Range(5, 40)] private int steps;
        [SerializeField][Range(10, 300)] private int minEnemies;
        [SerializeField][Range(50, 600)] private int maxEnemies;
        [SerializeField][Range(1, 2)] private float waveMultiplier;

        [Header("Player information")][SerializeField] private GameObject player;

        #endregion

        #region UI

        [SerializeField] private TMPro.TMP_Text textWave;
        [SerializeField] private TMPro.TMP_Text textRemaining;

        #endregion

        #region Internal

        private readonly System.Random random = new();
        private int waveIndex = 0;
        private EnemyWave currentWave;
        private float remainingTime = 0;
        private int tick = 0;
        private int nextStepCandidate = 0;
        private bool isActive = false;

        #endregion

        #region Getters

        public float CurrentTime => currentWave.Duration - remainingTime;

        #endregion

        public void NextWave()
        {
            enemyManager.Clear();
            waveIndex++;
        }

        public void StartWave()
        {
            currentWave = GetNextWave();
            textWave.SetText((waveIndex + 1).ToString());

            enemyManager.Clear();
            var quantities = currentWave.GetQuantities();
            remainingTime = currentWave.Duration;
            tick = 0;
            nextStepCandidate = 0;
            foreach (var quantity in quantities)
            {
                enemyManager.InitEnemySpecies(quantity.Builder, quantity.Quantity);
            }

            isActive = true;
        }

        void Start()
        {
            if (!TryGetComponent<EnemyManager>(out enemyManager))
            {
                enemyManager = gameObject.AddComponent<EnemyManager>();
                enemyManager.usedEnemiesTypes = 5;
                enemyManager.playerGameObject = player;
                enemyManager.Init();
            }

            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                if (player == null)
                {
                    Debug.LogError("Player not found");
                }
            }
        }

        void Update()
        {
            if (!isActive)
            {
                return;
            }

            remainingTime = Math.Max(0, remainingTime - Time.deltaTime);
            if (remainingTime == 0)
            {
                isActive = false;
                return;
            }

            // Only check next steps every x ticks
            if ((tick++ % sleepTicks) == 0)
            {
                textRemaining.SetText($"{Math.Round(remainingTime)}s");

                var nextStep = GetNextStep();
                if (nextStep == null)
                {
                    return;
                }

                StartWaveStep((EnemyWaveStep)nextStep);
                nextStepCandidate++;
            }
        }

        private EnemyWave GetNextWave()
        {
            if (waveIndex < waveCollection.Count)
            {
                return waveCollection.AtIndex(waveIndex);
            }

            float multiplier = (float)((waveIndex + 1) * waveMultiplier * random.NextDouble());
            EnemyBuilder[] enemyBuilders = { };
            return EnemyWaveBuilder.Random(duration, (int)Math.Round(steps * multiplier), (int)Math.Round(minEnemies * multiplier), (int)Math.Round(maxEnemies * multiplier), enemyBuilders);
        }

        private EnemyWaveStep? GetNextStep()
        {
            var steps = currentWave.Steps;
            if (steps.Length <= nextStepCandidate)
            {
                return null;
            }

            var candidate = steps[nextStepCandidate];
            if (CurrentTime >= candidate.Time)
            {
                return candidate;
            }

            return null;
        }

        private void StartWaveStep(EnemyWaveStep step)
        {
            enemyManager.Spawn(step.Builder, step.Quantity);
        }
    }
}