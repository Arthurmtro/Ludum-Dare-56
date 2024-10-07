using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField][Range(5, 120)] private float duration;
        [SerializeField][Range(5, 40)] private int steps;
        [SerializeField][Range(1, 300)] private int minEnemies;
        [SerializeField][Range(10, 600)] private int maxEnemies;
        [SerializeField][Range(1, 2)] private float waveMultiplier;
        [SerializeField] private EnemyController[] prefabs;

        [Header("Player information")]
        [SerializeField] private PlayerController player;

        #endregion

        #region UI

        [SerializeField] private TMPro.TMP_Text textWave;
        [SerializeField] private TMPro.TMP_Text textRemaining;

        #endregion

        #region Internal

        private readonly System.Random random = new();
        private int waveIndex = 0;
        public EnemyWave currentWave;
        private float remainingTime = 0;
        private int tick = 0;
        private int nextStepCandidate = 0;
        private bool isActive = false;

        #endregion

        #region Getters

        public float CurrentTime => currentWave.Duration - remainingTime;

        #endregion

        public void ClearWaves()
        {
            waveIndex = 0;
        }

        public void NextWave()
        {
            enemyManager.Clear();
            waveIndex++;
        }

        public void InitWave()
        {
            currentWave = GetNextWave();
            textWave.SetText($"WAVE {(waveIndex + 1)}");

            enemyManager.Clear();
            var quantities = currentWave.GetQuantities();
            remainingTime = currentWave.Duration;
            tick = 0;
            nextStepCandidate = 0;
            foreach (var quantity in quantities)
            {
                enemyManager.InitEnemySpecies(quantity.Prefab, quantity.Quantity);
            }
        }

        public void SetActive(bool value)
        {
            isActive = value;
            enemyManager.SetActive(value);
        }

        void Start()
        {
            if (!TryGetComponent<EnemyManager>(out enemyManager))
            {
                enemyManager = gameObject.AddComponent<EnemyManager>();
                enemyManager.usedEnemiesTypes = 5;
                enemyManager.player = player;
                enemyManager.Init();
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
                onWaveFinish.Invoke();
                return;
            }

            // Only check next steps every x ticks
            if ((tick++ % sleepTicks) == 0)
            {
                textRemaining.SetText($"{Math.Round(remainingTime)}");

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

            float multiplier = 1 + 0.1f * waveIndex;
            return EnemyWaveBuilder.Random(
                duration,
                steps,
                (int)Math.Round(minEnemies * multiplier),
                (int)Math.Round(maxEnemies * multiplier),
                prefabs);
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
            enemyManager.Spawn(step.Prefab, step.Quantity);
        }

        #region Events

        public readonly UnityEvent onWaveFinish = new();

        #endregion
    }
}