using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioClip SFXButtonPress;
    private AudioSource _audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ComponentRegister.AudioPlayer = this;
        _audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
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
        _audioSource.clip = SFXButtonPress;
        _audioSource.Play();
    }
}
