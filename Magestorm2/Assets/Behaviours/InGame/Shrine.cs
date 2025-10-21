using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Shrine : Trigger
{
    public Team Team;
    public BiasIndicator Indicator;
    private byte _health = 100;
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
        Match.RegisterShrine(this);
        Indicator.ChangeBias(Team);
        _health = MatchParams.GetShrineData()[(byte)Team - 1];
    }
    public override void EnterAction()
    {
        if (PlayerAccount.SelectedCharacter.CharacterClass != (byte)PlayerClass.Arcanist)
        {
            _playerInShrine = true;
            ComponentRegister.ShrineDisplay.Refresh(this);
        }
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
        _health = amount;
        Indicator.gameObject.SetActive(_health > 0);
        ComponentRegister.ShrinePanel.SetFill(Team, _health);
    }
    public void AdjustHealth(byte newHealth, byte adjusterID)
    {
        SetHealth(newHealth);
        Avatar adjuster = null;
        if (_health == 100)
        {
            ComponentRegister.Notifier.DisplayNotification(Language.BuildString(180, Teams.GetTeamName(Team))); //
        }
        else if (_health == 0)
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
            if(adjuster.PlayerID == MatchParams.IDinMatch)
            {
                if(MatchParams.MatchTeam == Team)
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
                if(adjuster.PlayerTeam == Team)
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
    public byte GetHealth()
    {
        return _health;
    }
    public void Update()
    {
        if (_playerInShrine)
        {
            _elapsed += Time.deltaTime;
            if (_elapsed > _interval)
            {
                _elapsed = 0.0f;
                if ((MatchParams.MatchTeamID == (byte)Team && _health < 100)
                    || (MatchParams.MatchTeamID != (byte)Team && _health > 0))
                {
                    Game.SendInGameBytes(InGame_Packets.AdjustShrinePacket((byte)Team));
                }
            }
        }
    }
}

