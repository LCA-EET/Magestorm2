using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class PC : MonoBehaviour
{
    public PlayerMovement PlayerMovement;
    public RayCaster DownwardCaster;
    public RayCaster ForwardCaster;
    private BoxCollider _playerCollider;

    public SFXPlayer SFXPlayer;
    public MusicPlayer MusicPlayer;
    public bool JoinedMatch;

    private bool _hpUpdating;
    private float _hpUpdateElapsed;
    private bool _manaUpdating;
    private float _manaUpdateElapsed;

    private float _hmlPeriod = 0.2f;

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
            _hpUpdateElapsed = 0.0f;
            _manaUpdateElapsed = 0.0f;
        }
    }
    public void Start()
    {
        _maxHP = PlayerAccount.SelectedCharacter.GetMaxHP();
        ComponentRegister.PlayerStatusPanel.SetIndicator(PlayerIndicator.Health, _currentHP / _maxHP);
    }
    private void UpdateIndication(PlayerIndicator toUpdate, ref float elapsed, float prior, float current, float max, ref bool updating)
    {
        float value = 0;
        if (SharedFunctions.ProcessFloatLerp(ref elapsed, _hmlPeriod, prior, current, ref value))
        {
            _hpUpdating = false;
        }
        ComponentRegister.PlayerStatusPanel.SetIndicator(toUpdate, value / max);
    }
    public void Update()
    {
        if (_hpUpdating)
        {
            UpdateIndication(PlayerIndicator.Health, ref _hpUpdateElapsed, _priorHP, _currentHP, _maxHP, ref _hpUpdating);
        }
        if (_manaUpdating)
        {
            UpdateIndication(PlayerIndicator.Mana, ref _manaUpdateElapsed, _priorMana, _currentMana, _maxMana, ref _manaUpdating);
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
        MenuCheck();
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
        float newHP = BitConverter.ToSingle(decrypted, 1);
        float newMana = BitConverter.ToSingle(decrypted, 5);
        HMLUpdate(newHP, ref _currentHP, ref _priorHP, ref _hpUpdating, ref _hpUpdateElapsed);
        HMLUpdate(newMana, ref _currentMana, ref _priorMana, ref _manaUpdating, ref _manaUpdateElapsed);
    }
    private void HMLUpdate(float newValue, ref float currentValue, ref float priorValue, ref bool updating, ref float elapsed)
    {
        if (!updating)
        {
            priorValue = currentValue;
        }
        currentValue = newValue;
        updating = currentValue != priorValue;
        if (updating)
        {
            elapsed = 0.0f;
        }
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
