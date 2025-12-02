using UnityEditor.Build;
using UnityEngine;

public class ManaPool : BiasableTrigger
{
    public byte PoolID;
    public LeyInfluencer LeyInfluencer;
    public BiasIndicator Indicator;
    private bool _playerInPool = false;
    private byte _poolPower;
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
    public void Start()
    {
        InitTrigger(TriggerType.ManaPool);
        if(ComponentRegister.PC.CharacterClass == PlayerClass.Magician)
        {
            LeyInfluencer.AssignOwner(this, _poolPower, PoolID);
        }
        else
        {
            Destroy(LeyInfluencer.gameObject);
        }
        new PeriodicAction(5.0f, BiasPool, _actionList);
        _poolPower = PoolManager.RegisterPool(this);
        Debug.Log("Pool ID: " + PoolID + ", Power: " + _poolPower);
    }
    private void BiasPool()
    {
        if ((MatchParams.MatchTeamID != (byte)BiasedToward) || (BiasAmount < 100))
        {
            Game.SendInGameBytes(InGame_Packets.BiasPoolPacket(PoolID));
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
                ComponentRegister.AudioPlayer.PlayBiasSound();
                ComponentRegister.PC.UseStamina(ComponentRegister.PC.CurrentStamina);
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
        }
    }

    public byte GetPoolPower()
    {
        return _poolPower;
    }
    public override void EnterAction()
    {
        if(PlayerAccount.SelectedCharacter.CharacterClass != (byte)PlayerClass.Arcanist)
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
}
