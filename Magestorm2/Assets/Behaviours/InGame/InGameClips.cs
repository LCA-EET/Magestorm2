using Unity.VisualScripting;
using UnityEngine;
public class InGameClips : MonoBehaviour
{
    public float FootstepAudioDistance;

    public AudioClip SFXBias;
    public AudioClip SFXDestroyedShrine;
    public AudioClip[] SFXWaterSplash;
    public AudioClip SFXFootstep_Stone;
    public AudioClip SFXFootstep_Wood;
    public AudioClip SFXFootstep_Grass;
    public AudioClip SFXFootstep_Dirt;
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
    private AudioClip GetRandomSplash()
    {
        return SFXWaterSplash[SharedFunctions.RandomInt(0, SFXWaterSplash.Length)];
    }
    public void PlayRandomSplash(AudioSource source)
    {
        source.PlayOneShot(GetRandomSplash());
    }
    public void PlayShrineDestroyed()
    {
        Game.UIAudio.PlayClip(SFXDestroyedShrine);
    }
    public void PlayBias(AudioSource source)
    {
        source.PlayOneShot(SFXBias);
    }
    public void PlayClip(AudioClip toPlay, AudioSource source, float maxDistance)
    {
        source.maxDistance = maxDistance;
        source.PlayOneShot(toPlay);
    }
    public void PlayFootstep(Footstep step, AudioSource source)
    {
        AudioClip toPlay = null;
        switch (step)
        {
            case Footstep.Stone:
                toPlay = SFXFootstep_Stone;
                break;
            case Footstep.Wood:
                toPlay = SFXFootstep_Wood;
                break;
            case Footstep.Dirt:
                toPlay = SFXFootstep_Dirt;
                break;
            case Footstep.Grass:
                toPlay = SFXFootstep_Grass;
                break;
            case Footstep.Water:
                toPlay = SFXWaterSplash[1];
                break;
        }
        PlayClip(toPlay, source, FootstepAudioDistance);
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
