using System.Collections;
using System.Collections.Generic;
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

        #region  Events

        public void Start()
        {
            waveManager.onWaveFinish.AddListener(OnWaveFinish);
            player.onDie.AddListener(OnPlayerDie);
        }

        public void OnPlayPress()
        {
            animator.SetInteger("Section", (int)GameSection.Game);
            animator.SetBool("GameUI", true);
            musicManager.SetQuiet(false);
            cameraFollow.SetBaseDistance(5f);
            waveManager.ClearWaves();
            waveManager.InitWave();
            waveManager.SetActive(true);
            player.entity.IsActive = true;
        }

        public void OnPausePress()
        {
            animator.SetBool("Pause", true);
            musicManager.SetQuiet(true);
            cameraFollow.SetBaseDistance(1.5f);
            waveManager.SetActive(false);
            player.entity.IsActive = false;
        }

        public void OnResumePress()
        {
            animator.SetBool("Pause", false);
            musicManager.SetQuiet(false);
            cameraFollow.SetBaseDistance(5f);
            waveManager.SetActive(true);
            player.entity.IsActive = true;
        }

        public void OnExitPress()
        {
            musicManager.SetQuiet(true);
            animator.SetBool("Pause", false);
            animator.SetBool("GameUI", false);
            animator.SetInteger("Section", (int)GameSection.Start);
            cameraFollow.SetBaseDistance(1.5f);
        }

        public void OnWaveFinish()
        {
            cameraFollow.SetBaseDistance(1.5f);
            animator.SetBool("GameUI", false);
            animator.SetInteger("Section", (int)GameSection.WaveFinish);
            effectManager.StartWaveParticles();
            player.entity.IsActive = false;
        }

        public void OnSelectMod1()
        {
            Debug.Log("Select Modifier 1");
            player.OnSpeedModifier();
            OnSelectMod(0);
        }
        public void OnSelectMod2()
        {
            Debug.Log("Select Modifier 2");
            player.OnAttackSpeedModifier();
            OnSelectMod(1);
        }
        public void OnSelectMod3()
        {
            Debug.Log("Select Modifier 3");
            player.OnAttackDamageModifier();
            OnSelectMod(2);
        }

        public void OnPlayerDie()
        {
            musicManager.SetQuiet(true);
            cameraFollow.SetBaseDistance(1.5f);
            animator.SetBool("GameUI", false);
            animator.SetInteger("Section", (int)GameSection.GameOver);
            waveManager.SetActive(false);
        }

        #endregion

        private void OnSelectMod(int position)
        {
            effectManager.StopWaveParticles();
            animator.SetInteger("Section", (int)GameSection.Game);
            animator.SetBool("GameUI", true);
            musicManager.SetQuiet(false);
            cameraFollow.SetBaseDistance(5f);
            waveManager.NextWave();
            waveManager.InitWave();
            waveManager.SetActive(true);
        }
    }
}