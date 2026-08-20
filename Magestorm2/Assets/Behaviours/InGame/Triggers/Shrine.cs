using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : BiasableTrigger
{
    
    public LeyInfluencer LeyInfluencer;
    public BiasIndicator Indicator;
    public byte ShrinePower = 100;
    private bool _playerInShrine = false;
    private Team _team;

    public Team Team
    {
        get { return _team; }
    }

    protected override void Awake()
    {
        if (!MatchParams.IncludeShrines)
        {
            Destroy(this);
        }
        else
        {
            base.Awake();
        }
    }
    public void AssignToTeam(Team team)
    {
        _team = team;
        BiasedToward = _team;
        ShrineManager.RegisterShrine(this);
        //BiasAmount = 100;
        InitTrigger(TriggerType.Shrine);
        if (ComponentRegister.PC.CharacterClass == ControlCodes.PlayerClass_Cleric)
        {
            Debug.Log("Assigning Team to Ley Influencer: " + _team.ToString());
            LeyInfluencer.AssignOwner(this, ShrinePower, (byte)_team);
        }
        else
        {
            Destroy(LeyInfluencer.gameObject);
        }
        new PeriodicAction(5.0f, BiasShrine, _actionList);
        Indicator.ChangeBias(Team);
    }
    public void Start()
    {
        
    }
    public override void EnterAction()
    {
        base.EnterAction();
        _playerInShrine = true;
        ComponentRegister.ShrineDisplay.Refresh(this);
    }
    public override void ExitAction()
    {
        base.ExitAction();
        _playerInShrine = false;
        ComponentRegister.ShrineDisplay.Toggle(false);
    }
   
    public void SetHealth(byte amount)
    {
        BiasAmount = amount;
        Indicator.gameObject.SetActive(BiasAmount > 0);
        if(amount == 0 || amount == 100)
        {
            ShrineManager.CheckVictoryCondition();
        }
        ComponentRegister.ShrinePanel.SetFill(Team, BiasAmount);
        TorchManager.AdjustTeamTorchIntensity(Team, BiasAmount / 100.0f);
    }
    public void AdjustHealth(byte newHealth, byte adjusterID)
    {
        SetHealth(newHealth);
        Avatar adjuster = null;
        if (BiasAmount == 100)
        {
            ComponentRegister.Notifier.DisplayNotification(Language.BuildString(180, Teams.GetTeamName(Team))); //
        }
        else if (BiasAmount == 0)
        {
            ComponentRegister.Notifier.DisplayNotification(Language.BuildString(179, Teams.GetTeamName(Team))); //
            
        }
        if (_playerInShrine)
        {
            ComponentRegister.ShrineDisplay.Refresh(this);
        }
        if (Match.PlayerExists(adjusterID, ref adjuster))
        {
            string notificationText = "";
            if (adjuster.PlayerID == MatchParams.IDinMatch)
            {
                if(BiasAmount > 0)
                {
                    if (MatchParams.MatchTeam == Team)
                    {
                        notificationText = Language.BuildString(175, Language.GetBaseString(177), Teams.GetTeamName(Team)); //
                    }
                    else
                    {
                        notificationText = Language.BuildString(175, Language.GetBaseString(178), Teams.GetTeamName(Team)); //
                    }
                }
                else
                {
                    notificationText = Language.BuildString(380, Teams.GetTeamName(Team));
                }
            }
            else
            {
                if(BiasAmount > 0)
                {
                    if (adjuster.PlayerTeam == Team)
                    {
                        notificationText = Language.BuildString(176, adjuster.Name, Language.GetBaseString(177), Teams.GetTeamName(Team)); //
                    }
                    else
                    {
                        notificationText = Language.BuildString(177, adjuster.Name, Language.GetBaseString(178), Teams.GetTeamName(Team)); //
                    }
                }
                else
                {
                    notificationText = Language.BuildString(381, adjuster.Name, Teams.GetTeamName(Team));
                }
            }
            if(BiasAmount == 0)
            {
                Game.Clips.PlayShrineDestroyed();
            }
            else
            {
                Game.Clips.PlayBias(adjuster.AudioSource);
            }
            ComponentRegister.Notifier.DisplayNotification(notificationText);
        }
    }
    private void BiasShrine()
    {
        if ((MatchParams.MatchTeamID == (byte)Team && BiasAmount < 100)
                    || (MatchParams.MatchTeamID != (byte)Team && BiasAmount > 0))
        {
            ComponentRegister.PC.UseStamina(255);
            Game.SendInGameBytes(InGame_Packets.AdjustShrinePacket((byte)Team));
        }
    }
    public void Update()
    {
        if (_playerInShrine)
        {
            PeriodicAction.PerformActions(Time.deltaTime, _actionList);
        }
    }
}

