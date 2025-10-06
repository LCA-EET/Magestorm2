using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioClip SFXButtonPress;
    public AudioClip SFXMessageNotification;
    public AudioClip SFXBias;
    private AudioSource _audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ComponentRegister.AudioPlayer = this;
        _audioSource = GetComponent<AudioSource>();
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayClip(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
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
