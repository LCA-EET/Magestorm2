using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : BiasableTrigger
{
    public Team Team;
    public LeyInfluencer LeyInfluencer;
    public BiasIndicator Indicator;
    public byte ShrinePower = 100;
    private bool _playerInShrine = false;

    public void Awake()
    {
        if (!MatchParams.IncludeShrines)
        {
            Destroy(this);
        }
    }
    public void Start()
    {
        ShrineManager.RegisterShrine(this);
        BiasAmount = 100;
        InitTrigger(TriggerType.Shrine);
        if(ComponentRegister.PC.CharacterClass == PlayerClass.Cleric)
        {
            LeyInfluencer.AssignOwner(this, ShrinePower, (byte)Team);
        }
        else
        {
            Destroy(LeyInfluencer.gameObject);
        }
        new PeriodicAction(5.0f, BiasShrine, _actionList);
        Indicator.ChangeBias(Team);
    }
    public override void EnterAction()
    {
        _playerInShrine = true;
        ComponentRegister.ShrineDisplay.Refresh(this);
        Debug.Log("Entered shrine");
    }
    public override void ExitAction()
    {
        _playerInShrine = false;
        ComponentRegister.ShrineDisplay.Toggle(false);
        Debug.Log("Exited shrine");
    }
    public Team GetTeam()
    {
        return Team;
    }
    public void SetHealth(byte amount)
    {
        BiasAmount = amount;
        Indicator.gameObject.SetActive(BiasAmount > 0);
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
                if (MatchParams.MatchTeam == Team)
                {
                    notificationText = Language.BuildString(175, Language.GetBaseString(177), Teams.GetTeamName(Team)); //
                }
                else
                {
                    notificationText = Language.BuildString(175, Language.GetBaseString(178), Teams.GetTeamName(Team)); //
                }
                ComponentRegister.AudioPlayer.PlayBiasSound();
            }
            else
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
            ComponentRegister.Notifier.DisplayNotification(notificationText);
        }
    }
    private void BiasShrine()
    {
        if ((MatchParams.MatchTeamID == (byte)Team && BiasAmount < 100)
                    || (MatchParams.MatchTeamID != (byte)Team && BiasAmount > 0))
        {
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

