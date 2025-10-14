using UnityEngine;

public class PC : MonoBehaviour
{
    public PlayerMovement PlayerMovement;
    private BoxCollider _playerCollider;

    public SFXPlayer SFXPlayer;
    public MusicPlayer MusicPlayer;
    public Avatar PCAvatar;

    private float _surfaceCheck = 0.1f;
    private float _surfaceCheckElapsed = 0.0f;

    private ManaPool _enteredPool;
    public void Awake()
    {
        ComponentRegister.PC = this;
        _playerCollider = GetComponent<BoxCollider>();
        PCAvatar.SetAttributes(MatchParams.IDinMatch, PlayerAccount.SelectedCharacter.CharacterName, PlayerAccount.SelectedCharacter.CharacterLevel, PlayerAccount.SelectedCharacter.CharacterClass, MatchParams.MatchTeam);
        //ComponentRegister.PCCollider = _playerCollider;
    }
    public void Start()
    {
        Match.AddAvatar(PCAvatar);
    }
    public void Update()
    {
        MenuCheck();
        _surfaceCheckElapsed += Time.deltaTime;
        if(_surfaceCheckElapsed > _surfaceCheck)
        {

        }
    }
    private void MenuCheck()
    {
        if (InputControls.InGameMenu && !Game.ControlMode)
        {
            if (!Game.MenuMode)
            {
                ComponentRegister.UIPrefabManager.InstantiateInGameMenu();
            }
            else
            {
                ComponentRegister.UIPrefabManager.PopFromStack();
            }
            Game.MenuMode = !Game.MenuMode;
            Debug.Log("Menu Mode? " + Game.MenuMode);
        }
    }
    public void PlaySFX(AudioClip clip)
    {
        SFXPlayer.PlayClip(clip);
    }

    public void OnTriggerEnter(Collider other)
    {
        Trigger toProcess = null;
        if (ObtainTrigger(other, ref toProcess))
        {
            toProcess.EnterAction();
        }
    }
    public void OnTriggerExit(Collider other)
    {
        Trigger toProcess = null;
        if (ObtainTrigger(other, ref toProcess))
        {
            toProcess.ExitAction();
        }
    }
    private bool ObtainTrigger(Collider other, ref Trigger trigger)
    {
        trigger = other.GetComponent<Trigger>();
        return trigger != null;
    }
}
