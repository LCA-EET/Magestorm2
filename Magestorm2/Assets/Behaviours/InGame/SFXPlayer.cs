using Unity.VisualScripting;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioClip SFXButtonPress;
    public AudioClip SFXMessageNotification;
    public AudioClip SFXBias;
    public AudioClip SFXDestroyedShrine;
    public AudioClip[] SFXWaterSplash;
    public AudioClip SFXWoosh_HeavyFast;
    public AudioClip SFXWoosh_HeavyMedium;
    public AudioClip SFXWoosh_HeavySlow;
    public AudioClip SFXWoosh_NormalFast;
    public AudioClip SFXWoosh_NormalMedium;
    public AudioClip SFXWoosh_NormalSlow;

    private AudioSource[] _audioSources;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(ComponentRegister.AudioPlayer != null)
        {
            Destroy(ComponentRegister.AudioPlayer.gameObject);
        }
        ComponentRegister.AudioPlayer = this;
        _audioSources = GetComponents<AudioSource>();
        
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayWoosh(Woosh toPlay)
    {
        AudioClip clipToPlay = null;
        switch (toPlay)
        {
            case Woosh.HeavyFast:
                clipToPlay = SFXWoosh_HeavyFast;
                break;
            case Woosh.HeavyMedium:
                clipToPlay = SFXWoosh_HeavyMedium;
                break;
            case Woosh.HeavySlow:
                clipToPlay = SFXWoosh_HeavySlow;
                break;
            case Woosh.NormalFast:
                clipToPlay = SFXWoosh_NormalFast;
                break;
            case Woosh.NormalMedium:
                clipToPlay = SFXWoosh_NormalMedium;
                break;
            case Woosh.NormalSlow:
                clipToPlay = SFXWoosh_NormalSlow;
                break;

        }
        if (clipToPlay != null)
        {
            PlayClip(clipToPlay);
        }
    }
    public void PlayWaterSplash()
    {
        int index = SharedFunctions.RandomInt(0, SFXWaterSplash.Length);
        PlayClip(SFXWaterSplash[index]);
    }
    public void PlayClip(AudioClip clip)
    {
        for(int i = 0; i < _audioSources.Length; i++)
        {
            AudioSource source = _audioSources[i];
            if (!source.IsDestroyed() && !source.isPlaying)
            {
                source.clip = clip;
                source.Play();
                break;
            }
        }
    }
  
    public void PlayButtonPress()
    {
        PlayClip(SFXButtonPress);
    }
    public void PlayNotificationSound()
    {
        PlayClip(SFXMessageNotification);
    }

    public void PlayShrineDestruction()
    {
        PlayClip(SFXDestroyedShrine);
    }
    public void PlayBiasSound()
    {
        PlayClip(SFXBias);
    }
}
