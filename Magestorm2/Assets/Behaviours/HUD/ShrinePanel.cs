using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ShrinePanel : MonoBehaviour
{
    public BarIndicator[] ShrineIndicators;
    public TMP_Text VictoryNotification;
    
    private Dictionary<Team, BarIndicator> _indicators;

    private void Awake()
    {
        if (!MatchParams.IncludeShrines)
        {
            Destroy(gameObject);
        }
        else
        {
            ComponentRegister.ShrinePanel = this;
            _indicators = new Dictionary<Team, BarIndicator>();
            _indicators.Add(Team.Chaos, ShrineIndicators[0]);
            _indicators.Add(Team.Balance, ShrineIndicators[1]);
            _indicators.Add(Team.Order, ShrineIndicators[2]);
        }
    }
    public void HideVictoryNotification()
    {
        VictoryNotification.gameObject.SetActive(false);
    }
    public void ShowVictoryNotification(Team winning)
    {
        string text = Language.BuildString(382, Teams.GetTeamName(winning));
        VictoryNotification.text = text;
        VictoryNotification.gameObject.SetActive(true);
    }
    void Start()
    {
        Teams.Init();
        foreach (Team key in _indicators.Keys)
        {
            _indicators[key].SetFillColor(Teams.GetTeamColor(key));
        }
    }
    public void SetFill(Team team, byte health)
    {
        _indicators[team].SetFill(health / 100f);
    }
}
