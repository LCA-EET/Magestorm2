using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PlayerStatusPanel : MonoBehaviour
{
    public BarIndicator[] PlayerIndicators;
    public TMP_Text PlayerName;
    public TMP_Text LCT;
    public Counter Kills;
    public Counter Deaths;
    private Dictionary<PlayerIndicator, BarIndicator> _indicators;
    private Dictionary<PlayerIndicator, Color> _indicatorColors;
    private PlayerCharacter _pc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pc = PlayerAccount.SelectedCharacter;
        _indicators = new Dictionary<PlayerIndicator, BarIndicator>();
        _indicators.Add(PlayerIndicator.Health, PlayerIndicators[0]);
        _indicators.Add(PlayerIndicator.Mana, PlayerIndicators[1]);
        _indicators.Add(PlayerIndicator.Ley, PlayerIndicators[2]);
        _indicators.Add(PlayerIndicator.Stamina, PlayerIndicators[3]);
        _indicatorColors = new Dictionary<PlayerIndicator, Color>();
        _indicatorColors.Add(PlayerIndicator.Health, Color.red);
        _indicatorColors.Add(PlayerIndicator.Mana, Color.blue);
        _indicatorColors.Add(PlayerIndicator.Ley, Color.gray);
        _indicatorColors.Add(PlayerIndicator.Stamina, Color.yellow);
        foreach (PlayerIndicator indicator in _indicators.Keys)
        {
            _indicators[indicator].SetFillColor(_indicatorColors[indicator]);
        }
        ComponentRegister.PlayerStatusPanel = this;
        Teams.Init();
        SetPlayerName(_pc.CharacterName);
        SetLCT(_pc.CharacterLevel, (PlayerClass)_pc.CharacterClass, (Team)MatchParams.MatchTeamID);
        SetIndicator(PlayerIndicator.Health, 0.9f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetIndicator(PlayerIndicator indic, float fill)
    {
        _indicators[indic].SetFill(fill);
    }
    public void SetPlayerName(string name)
    {
        PlayerName.text = name;
    }
    public void SetLCT(byte level, PlayerClass playerClass, Team team)
    {
        string levelString = Language.GetBaseString(13);
        string ofString = Language.GetBaseString(14);
        string baseString = team == Team.Neutral ? levelString + " {0} {1}" : levelString + " {0} {1} " + ofString + " {2}";
        if (team == Team.Neutral)
        {
            baseString = string.Format(baseString, level, SharedFunctions.PlayerClassToString(playerClass));
        }
        else
        {
            baseString = string.Format(baseString, level, SharedFunctions.PlayerClassToString(playerClass), Teams.GetTeamName(team));
        }
        LCT.text = baseString;
    }
    public void SetKillCounter(byte killCount)
    {
        Kills.SetCount(killCount);
    }
    public void SetDeathCounter(byte deathCount)
    {
        Deaths.SetCount(deathCount);   
    }
}
