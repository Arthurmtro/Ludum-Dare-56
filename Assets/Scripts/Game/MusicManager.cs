using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MusicManager : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private AudioClip[] clips;
    [SerializeField][Range(0, 10)] private int level = 0;
    [SerializeField] private float duration;
    [SerializeField] private float tickDuration;
    [SerializeField] private bool isQuiet = false;
    [SerializeField] private float maxVolume = 0.5f;

    private AudioSource[] sources = Array.Empty<AudioSource>();
    private float[] targetVolumes = Array.Empty<float>();
    private float prevTime;

    #region Getters
    float MaxLevel => isQuiet ? 0.25f : maxVolume;
    #endregion

    #region Setters

    public void SetLevel(int newLevel)
    {
        level = newLevel;
    }
    public void SetQuiet(bool newValue)
    {
        this.isQuiet = newValue;
    }

    #endregion

    void Start()
    {
        sources = new AudioSource[clips.Length];
        targetVolumes = new float[clips.Length];
        for (int i = 0; i < clips.Length; i++)
        {
            var component = gameObject.AddComponent<AudioSource>();
            component.volume = level >= i ? MaxLevel : 0;
            targetVolumes[i] = level >= i ? MaxLevel : 0;
            component.playOnAwake = true;
            component.loop = true;
            component.clip = clips[i];
            component.Play();
            sources[i] = component;
        }
    }

    void Update()
    {
        float time = sources[0].time;
        if (time % tickDuration < prevTime % tickDuration)
        {
            OnTick();
        }
        if (time < prevTime)
        {
            OnLoop();
        }

        prevTime = time;
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].volume = Mathf.Lerp(sources[i].volume, targetVolumes[i], Time.deltaTime * 2.5f);
        }
    }

    private void OnTick()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            targetVolumes[i] = level >= i ? MaxLevel : 0;
        }
    }

    private void OnLoop()
    {
    }
}
