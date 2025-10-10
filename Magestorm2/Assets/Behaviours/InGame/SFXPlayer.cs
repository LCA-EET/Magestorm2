using Unity.VisualScripting;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioClip SFXButtonPress;
    public AudioClip SFXMessageNotification;
    public AudioClip SFXBias;
    private AudioSource _audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(ComponentRegister.AudioPlayer != null)
        {
            Destroy(ComponentRegister.AudioPlayer.gameObject);
        }
        ComponentRegister.AudioPlayer = this;
        _audioSource = GetComponent<AudioSource>();
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
        if (!_audioSource.IsDestroyed())
        {
            _audioSource.clip = clip;
            _audioSource.Play();
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
