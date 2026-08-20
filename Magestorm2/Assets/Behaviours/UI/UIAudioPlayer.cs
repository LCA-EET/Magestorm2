using UnityEngine;
public class UIAudioPlayer : MonoBehaviour
{
    public AudioClip SFXButtonPress;
    public AudioClip SFXMessageNotification;

    public AudioSource AudioSource2D;

    public void Awake()
    {
        Game.UIAudio = this;
    }
    public void PlayClip(AudioClip clip)
    {
        AudioSource2D.PlayOneShot(clip);
    }

    public void PlayButtonPress()
    {
        PlayClip(SFXButtonPress);
    }
    public void PlayNotificationSound()
    {
        PlayClip(SFXMessageNotification);
    }
}
