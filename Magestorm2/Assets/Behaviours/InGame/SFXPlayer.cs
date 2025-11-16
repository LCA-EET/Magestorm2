using Unity.VisualScripting;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioClip SFXButtonPress;
    public AudioClip SFXMessageNotification;
    public AudioClip SFXBias;
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

    public void PlayBiasSound()
    {
        PlayClip(SFXBias);
    }
}
