using UnityEngine;

public class Player : MonoBehaviour
{
    private AudioSource _audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        ComponentRegister.PlayerAudioSource = _audioSource;
        ComponentRegister.Player = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (InputControls.InGameMenu)
        {
            if (!Match.MenuMode)
            {
                ComponentRegister.UIPrefabManager.InstantiateInGameMenu();
            }
            else
            {
                ComponentRegister.UIPrefabManager.PopFromStack();
            }
            Match.MenuMode = !Match.MenuMode;
        }
    }

    public void PlayAudioClip(AudioClip toPlay)
    {
        if (toPlay != null)
        {
            _audioSource.PlayOneShot(toPlay);
            //Debug.Log("Play clip: " + toPlay.name);
        }
    }
}
