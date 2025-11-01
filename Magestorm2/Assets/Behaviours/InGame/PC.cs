using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PC : MonoBehaviour
{
    public PlayerMovement PlayerMovement;
    public RayCaster DownwardCaster;
    public RayCaster ForwardCaster;
    private BoxCollider _playerCollider;

    public SFXPlayer SFXPlayer;
    public MusicPlayer MusicPlayer;
    public bool JoinedMatch;

    private float _hmlCheckInterval = 0.1f;
    private float _hmlCheckElapsed = 0.0f;

    private float _maxHP, _maxMana;
    private float _currentHP, _currentMana;  
    private float _priorHP, _priorMana;

    private ManaPool _enteredPool;
    public void Awake()
    {
        if (!Game.Running)
        {
            SceneManager.LoadScene("Pregame");
        }
        else
        {
            ComponentRegister.PC = this;
            PlayerMovement.SetPC(this);
            _playerCollider = GetComponent<BoxCollider>();
        }
    }
    public void Start()
    {
    }
    public void Update()
    {
        MenuCheck();
        if (_hmlCheckElapsed >= _hmlCheckInterval)
        {
            _hmlCheckElapsed = 0.0f;
            HMLCheck();
        }
        else
        {
            _hmlCheckElapsed += Time.deltaTime;
        }
        if (IsAlive && InputControls.Action)
        {
            Activate();
        }
    }
    private void HMLCheck()
    {

        bool updateNeeded = false;
        if (_priorHP != _currentHP || _priorMana != _currentMana)
        {
            updateNeeded = true;
        }
        if (updateNeeded)
        {
            _priorHP = _currentHP;
            _priorMana = _currentMana;
            ComponentRegister.PlayerStatusPanel.SetIndicator(PlayerIndicator.Health, _currentHP / _maxHP * 1.0f);
            ComponentRegister.PlayerStatusPanel.SetIndicator(PlayerIndicator.Mana, _currentMana / _maxMana * 1.0f);
            updateNeeded = false;
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

    public void HMLUpdate(byte[] decrypted)
    {
        HMLUpdate(BitConverter.ToInt16(decrypted, 1), BitConverter.ToInt16(decrypted, 3));
        
    }

    public void HMLUpdate(short hp, short mana)
    {
        _currentHP = hp;
        _currentMana = mana;
    }

    public bool IsAlive
    {
        get
        {
            return _currentHP > 0;
        }
    }

    private void Activate()
    {
        RaycastHit hitInfo;
        if (ForwardCaster.CastForward(LayerManager.InteractableMask, 1.0f, out hitInfo))
        {
            Debug.Log(hitInfo.collider.name);
            hitInfo.collider.gameObject.GetComponent<ActivateableObject>().StateChangeRequest();
        }
    }
}
