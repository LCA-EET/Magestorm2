using UnityEngine;
public class InGameClips : MonoBehaviour
{
    public AudioClip SFXBias;
    public AudioClip SFXDestroyedShrine;
    public AudioClip[] SFXWaterSplash;
    public AudioClip SFXWoosh_HeavyFast;
    public AudioClip SFXWoosh_HeavyMedium;
    public AudioClip SFXWoosh_HeavySlow;
    public AudioClip SFXWoosh_NormalFast;
    public AudioClip SFXWoosh_NormalMedium;
    public AudioClip SFXWoosh_NormalSlow;

    public void Awake()
    {
        Game.Clips = this;
    }
    public void PlayRandomSplash(AudioSource source)
    {
        int index = SharedFunctions.RandomInt(0, SFXWaterSplash.Length);
        source.PlayOneShot(SFXWaterSplash[index]);
    }
    public void PlayShrineDestroyed()
    {
        Game.UIAudio.PlayClip(SFXDestroyedShrine);
    }
    public void PlayBias(AudioSource source)
    {
        source.PlayOneShot(SFXBias);
    }
    public void PlayClip(AudioClip toPlay, AudioSource source)
    {
        source.PlayOneShot(toPlay);
    }

    public void PlayWoosh(Woosh woosh, AudioSource source)
    {
        AudioClip toPlay = null;
        switch (woosh)
        {
            case Woosh.NormalFast:
                toPlay = SFXWoosh_NormalFast;
                break;
            case Woosh.NormalMedium:
                toPlay = SFXWoosh_NormalMedium;
                break;
            case Woosh.NormalSlow:
                toPlay = SFXWoosh_NormalSlow;
                break;
            case Woosh.HeavyFast:
                toPlay = SFXWoosh_HeavyFast;
                break;
            case Woosh.HeavyMedium:
                toPlay = SFXWoosh_HeavyFast;
                break;
            case Woosh.HeavySlow:
                toPlay = SFXWoosh_HeavyFast;
                break;
        }
        if(toPlay != null)
        {
            source.PlayOneShot(toPlay);
        }
    }
}
