using System;
using UnityEngine;

public class ManaPool : BiasableTrigger, IComparable<ManaPool>
{
    public GameObject WaterPlane;
    public byte PoolPower;
    private byte _poolID;
    public LeyInfluencer LeyInfluencer;
    public BiasIndicator Indicator;
    private bool _playerInPool = false;
    protected override void Awake()
    {
        if (!MatchParams.IncludePools)
        {
            Destroy(this);
        }
        else
        {
            base.Awake();
        }
    }
    public byte PoolID
    {
        get
        {
            return _poolID;
        }
    }
    public void Start()
    {
        InitTrigger(TriggerType.ManaPool);
        if (ComponentRegister.PC.CharacterClass != ControlCodes.PlayerClass_Arcanist)
        {
            new PeriodicAction(5.0f, BiasPool, _actionList);
        }
        Vector3 wpScale = gameObject.transform.localScale * 0.1f;
        WaterPlane.transform.localScale = wpScale;
        WaterPlane.gameObject.SetActive(true);

    }
    public void RegisterPool(byte poolID)
    {
        _poolID = poolID;
        PoolManager.RegisterPool(this);
        Debug.Log("Registered Pool " + _poolID);
        if (ComponentRegister.PC.CharacterClass == ControlCodes.PlayerClass_Magician)
        {
            LeyInfluencer.AssignOwner(this, PoolPower, PoolID);
        }
        else
        {
            Destroy(LeyInfluencer.gameObject);
        }
    }
    private void BiasPool()
    {
        if ((MatchParams.MatchTeamID != (byte)BiasedToward) || (BiasAmount < 100))
        {
            Game.SendInGameBytes(InGame_Packets.BiasPoolPacket(PoolID));
            ComponentRegister.PC.UseStamina(ComponentRegister.PC.CurrentStamina);
            Debug.Log("Bias packet sent.");
        }
    }
    public void Update()
    {
        if (_playerInPool)
        {
            PeriodicAction.PerformActions(Time.deltaTime, _actionList);
        }
    }
    public void SetBiasAmount(byte amount, Team team)
    {
        BiasAmount = amount;
        BiasedToward = team;
        Indicator.ChangeBias(team);
        if (_playerInPool)
        {
            ComponentRegister.BiasDisplay.Refresh(this);
        }
    }
    public void BiasPool(byte amount, Team team, byte biaserID)
    {
        SetBiasAmount(amount, team);
        Avatar biaser = null;
        if(Match.PlayerExists(biaserID, ref biaser))
        {
            string notificationText = "";
            //increased bias
            if(biaserID == MatchParams.IDinMatch)
            {
                switch (team)
                {
                    case Team.Order:
                        notificationText = Language.GetBaseString(BiasedToward == biaser.PlayerTeam ? 158: 159);
                        break;
                    case Team.Chaos:
                        notificationText =  Language.GetBaseString(BiasedToward == biaser.PlayerTeam ? 162: 163); 
                        break;
                    case Team.Balance:
                        notificationText =  Language.GetBaseString(BiasedToward == biaser.PlayerTeam ? 160: 161); 
                        break;
                }
            }
            else
            {
                switch (team)
                {
                    case Team.Order:
                        notificationText = Language.BuildString(BiasedToward == biaser.PlayerTeam ? 164: 165, biaser.Name);
                        break;
                    case Team.Chaos:
                        notificationText = Language.BuildString(BiasedToward == biaser.PlayerTeam ? 168: 169, biaser.Name); 
                        break;
                    case Team.Balance:
                        notificationText =  Language.BuildString(BiasedToward == biaser.PlayerTeam ? 166: 167, biaser.Name); 
                        break;
                }
            }
            ComponentRegister.Notifier.DisplayNotification(notificationText);
            Game.Clips.PlayBias(biaser.AudioSource);
        }

    }
    public override void EnterAction()
    {
        if(PlayerAccount.SelectedCharacter.CharacterClass != ControlCodes.PlayerClass_Arcanist)
        {
            base.EnterAction();
            _playerInPool = true;
            ComponentRegister.BiasDisplay.Refresh(this);
        }
    }
    public override void ExitAction()
    {
        base.ExitAction();
        _playerInPool = false;
        ComponentRegister.BiasDisplay.Toggle(false);
    }

    public int CompareTo(ManaPool other)
    {
        return SharedFunctions.CompareVectors(transform.position, other.transform.position);
    }
}
