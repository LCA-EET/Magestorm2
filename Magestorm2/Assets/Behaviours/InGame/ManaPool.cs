using UnityEditor.Build;
using UnityEngine;

public class ManaPool : Trigger
{
    public byte PoolID;
    public BiasIndicator Indicator;
    private bool _playerInPool = false;
    private byte _poolPower;
    private Team _biasedToward;
    private byte _biasAmount;
    public void Awake()
    {
        
    }
    public void Start()
    {
        _poolPower = Match.RegisterPool(this);
        Debug.Log("Pool ID: " + PoolID + ", Power: " + _poolPower);
    }
    public void Update()
    {
        if (_playerInPool)
        {
            _elapsed += Time.deltaTime;
            if (_elapsed > _interval)
            {
                _elapsed = 0.0f;
                if((MatchParams.MatchTeamID != (byte)_biasedToward) || (_biasAmount < 100))
                {
                    Game.SendInGameBytes(InGame_Packets.BiasPoolPacket(PoolID));
                    Debug.Log("Bias packet sent.");
                }
            }
        }
    }
    public void SetBiasAmount(byte amount, Team team)
    {
        _biasAmount = amount;
        _biasedToward = team;
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
            if(_biasedToward == biaser.PlayerTeam)
            {
                //increased bias
                if(biaserID == MatchParams.IDinMatch)
                {
                    switch (team)
                    {
                        case Team.Order:
                            notificationText = Language.GetBaseString(157);
                            break;
                        case Team.Chaos:
                            notificationText = Language.GetBaseString(161);
                            break;
                        case Team.Balance:
                            notificationText = Language.GetBaseString(159);
                            break;
                    }
                    ComponentRegister.AudioPlayer.PlayBiasSound();
                }
                else
                {
                    switch (team)
                    {
                        case Team.Order:
                            notificationText = Language.BuildString(163, biaser.Name);
                            break;
                        case Team.Chaos:
                            notificationText = Language.BuildString(167, biaser.Name);
                            break;
                        case Team.Balance:
                            notificationText = Language.BuildString(165, biaser.Name);
                            break;
                    }
                }
            }
            else
            {
                if (biaserID == MatchParams.IDinMatch)
                {
                    switch (team)
                    {
                        case Team.Order:
                            notificationText = Language.GetBaseString(158);
                            break;
                        case Team.Chaos:
                            notificationText = Language.GetBaseString(162);
                            break;
                        case Team.Balance:
                            notificationText = Language.GetBaseString(160);
                            break;
                    }
                    ComponentRegister.AudioPlayer.PlayBiasSound();
                }
                else
                {
                    switch (team)
                    {
                        case Team.Order:
                            notificationText = Language.BuildString(164, biaser.Name);
                            break;
                        case Team.Chaos:
                            notificationText = Language.BuildString(168, biaser.Name);
                            break;
                        case Team.Balance:
                            notificationText = Language.BuildString(166, biaser.Name);
                            break;
                    }
                }

            }
            ComponentRegister.Notifier.DisplayNotification(notificationText);
        }
    }

    public byte GetBiasAmount()
    {
        return _biasAmount;
    }

    public Team GetTeam()
    {
        return _biasedToward;
    }

    public byte GetPoolPower()
    {
        return _poolPower;
    }
    public override void EnterAction()
    {
        if(PlayerAccount.SelectedCharacter.CharacterClass != (byte)PlayerClass.Arcanist)
        {
            _playerInPool = true;
            ComponentRegister.BiasDisplay.Refresh(this);
        }
        Debug.Log("Entered pool");
    }
    public override void ExitAction()
    {
        _playerInPool = false;
        ComponentRegister.BiasDisplay.Toggle(false);
        Debug.Log("Exited pool");
    }
}
