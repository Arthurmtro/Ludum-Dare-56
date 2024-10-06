using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Germinator
{
    public enum GameSection
    {
        Start = 0,
        Game = 1,
        WaveFinish = 2,
        GameOver = 3,
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private MusicManager musicManager;
        [SerializeField] private EnemyWaveManager waveManager;
        [SerializeField] private EffectsManager effectManager;
        [SerializeField] private CameraFollow cameraFollow;
        [SerializeField] private PlayerController player;
        [SerializeField] private GameUI gameUI;

        #region Score

        [SerializeField][Range(1, 60)] private float comboDuration = 10;
        private int kills = 0;
        private int comboLevel = 0;
        private float comboRemaining = 0;
        private int comboKills = 0;
        private int score = 0;

        #endregion

        #region  Events

        public bool IsActive { get; set; }

        public void Start()
        {
            waveManager.onWaveFinish.AddListener(OnWaveFinish);
            player.onKill.AddListener(OnPlayerKills);
            player.onHit.AddListener(OnPlayerHit);
            player.onDie.AddListener(OnPlayerDie);
        }

        void Update()
        {
            if (!IsActive)
            {
                return;
            }

            comboRemaining -= Time.deltaTime;
            if (comboRemaining <= 0)
            {
                ComboLevelDown();
            }
        }

        public void OnPlayPress()
        {
            kills = 0;
            score = 0;
            comboLevel = 0;
            comboRemaining = comboDuration;
            player.Clear();

            animator.SetInteger("Section", (int)GameSection.Game);
            animator.SetBool("GameUI", true);
            musicManager.SetQuiet(false);
            cameraFollow.SetBaseDistance(5f);
            waveManager.ClearWaves();
            waveManager.InitWave();
            waveManager.SetActive(true);
            player.entity.IsActive = true;
            gameUI.UpdatePlayer(player.entity);
            gameUI.UpdateScore(kills, score);
            gameUI.UpdateCombo(comboLevel);
            UpdateCameraFollow();
        }

        public void OnPausePress()
        {
            IsActive = false;
            animator.SetBool("Pause", true);
            musicManager.SetQuiet(true);
            cameraFollow.SetBaseDistance(1.5f);
            waveManager.SetActive(false);
            player.entity.IsActive = false;
        }

        public void OnResumePress()
        {
            IsActive = true;
            animator.SetBool("Pause", false);
            musicManager.SetQuiet(false);
            cameraFollow.SetBaseDistance(5f);
            waveManager.SetActive(true);
            player.entity.IsActive = true;
        }

        public void OnExitPress()
        {
            IsActive = false;
            musicManager.SetQuiet(true);
            musicManager.SetLevel(1);
            animator.SetBool("Pause", false);
            animator.SetBool("GameUI", false);
            animator.SetInteger("Section", (int)GameSection.Start);
            cameraFollow.SetBaseDistance(1.5f);
        }

        public void OnWaveFinish()
        {
            IsActive = false;
            cameraFollow.SetBaseDistance(1.5f);
            animator.SetBool("GameUI", false);
            animator.SetInteger("Section", (int)GameSection.WaveFinish);
            effectManager.StartWaveParticles();
            waveManager.SetActive(false);
            player.entity.IsActive = false;
        }

        public void OnSelectMod1()
        {
            player.OnSpeedModifier();
            OnSelectMod(0);
        }
        public void OnSelectMod2()
        {
            player.OnAttackSpeedModifier();
            OnSelectMod(1);
        }
        public void OnSelectMod3()
        {
            player.OnAttackDamageModifier();
            OnSelectMod(2);
        }

        public void OnPlayerKills()
        {
            kills++;
            comboKills++;
            score += 10 * comboLevel;

            if (comboKills >= 10 * comboLevel)
            {
                ComboLevelUp();
            }

            gameUI.UpdateScore(kills, score);
        }

        public void OnPlayerHit()
        {
            gameUI.UpdatePlayer(player.entity);
            player.ChangeBodyColor(Color.red, 1f);
        }

        public void OnPlayerDie()
        {
            IsActive = false;
            musicManager.SetQuiet(true);
            cameraFollow.SetBaseDistance(1.5f);
            animator.SetBool("GameUI", false);
            animator.SetInteger("Section", (int)GameSection.GameOver);
            waveManager.SetActive(false);
        }

        #endregion

        private void ComboLevelUp()
        {
            comboLevel++;
            comboRemaining = comboDuration;
            comboKills = 0;
            musicManager.SetLevel(comboLevel - 1);
            gameUI.UpdateCombo(comboLevel);
            UpdateCameraFollow();
        }

        private void ComboLevelDown()
        {
            comboRemaining = comboDuration;
            comboKills = 0;
            if (comboLevel <= 0)
            {
                return;
            }

            comboLevel--;
            musicManager.SetLevel(comboLevel - 1);
            gameUI.UpdateCombo(comboLevel);
            UpdateCameraFollow();
        }

        private void OnSelectMod(int position)
        {
            IsActive = true;
            effectManager.StopWaveParticles();
            animator.SetInteger("Section", (int)GameSection.Game);
            animator.SetBool("GameUI", true);
            musicManager.SetQuiet(false);
            cameraFollow.SetBaseDistance(5f);
            waveManager.NextWave();
            waveManager.InitWave();
            waveManager.SetActive(true);
            player.entity.IsActive = true;
        }

        private void UpdateCameraFollow()
        {
            cameraFollow.zoomOscillationSpeed = 6f + comboLevel;
            cameraFollow.oscillationSpeed = 3f + comboLevel;
        }
    }
}