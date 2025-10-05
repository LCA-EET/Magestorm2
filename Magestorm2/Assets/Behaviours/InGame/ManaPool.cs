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
    public void BiasPool(byte amount, Team team)
    {
        _biasAmount = amount;
        _biasedToward = team;
        Indicator.ChangeBias(team);
    }

    public byte GetBiasAmount()
    {
        return _biasAmount;
    }

    public Team GetTeam()
    {
        return _biasedToward;
    }

    public override void EnterAction()
    {
        if(PlayerAccount.SelectedCharacter.CharacterClass != (byte)PlayerClass.Arcanist)
        {
            _playerInPool = true;
        }
        Debug.Log("Entered pool");
    }
    public override void ExitAction()
    {
        _playerInPool = false;
        Debug.Log("Exited pool");
    }
}
