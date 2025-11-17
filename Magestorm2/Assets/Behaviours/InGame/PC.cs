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
    private float _prElapsed = 0.0f;
    private Vector3 _priorPosition, _priorRotation;
    

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
            _currentHP = 1;
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
        if(_prElapsed >= Game.TickInterval)
        {
            ReportMovement();
        }
        else
        {
            _prElapsed += Time.deltaTime;
        }
        
    }
    private void ReportMovement()
    {
        _prElapsed -= Game.TickInterval;
        if (transform.position != _priorPosition && transform.eulerAngles != _priorRotation)
        {
            _priorPosition = transform.position;
            _priorRotation = transform.eulerAngles;
            byte[] prData = new byte[28];
            ByteUtils.FillArray(ref prData, 0, _priorPosition);
            ByteUtils.FillArray(ref prData, 12, _priorRotation);
            Game.SendInGameBytes(InGame_Packets.PlayerMovedPacket(2, prData));
        }
        else if (transform.position != _priorPosition)
        {
            _priorPosition = transform.position;
            Game.SendInGameBytes(InGame_Packets.PlayerMovedPacket(0, ByteUtils.Vector3ToBytes(_priorPosition)));
        }
        else if (transform.eulerAngles != _priorRotation)
        {
            _priorRotation = transform.eulerAngles;
            Game.SendInGameBytes(InGame_Packets.PlayerMovedPacket(1, ByteUtils.Vector3ToBytes(_priorRotation)));
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
            ComponentRegister.PlayerStatusPanel.SetIndicator(PlayerIndicator.Health, _currentHP / _maxHP);
            ComponentRegister.PlayerStatusPanel.SetIndicator(PlayerIndicator.Mana, _currentMana / _maxMana);
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
        HMLUpdate(BitConverter.ToSingle(decrypted, 1), BitConverter.ToSingle(decrypted, 5));
        
    }

    public void HMLUpdate(float hp, float mana)
    {
        Debug.Log("HMLUpdate: HP: " + hp + ", Mana: " + mana);
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
        Debug.Log("Casting activation ray.");
        if (ForwardCaster.CastForward(LayerManager.InteractableMask, 2.0f, out hitInfo))
        {
            Debug.Log(hitInfo.collider.name);
            hitInfo.collider.gameObject.GetComponent<ActivateableObject>().StateChangeRequest();
        }
    }
}
