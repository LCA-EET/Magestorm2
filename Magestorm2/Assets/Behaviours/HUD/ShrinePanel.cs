using System.Collections.Generic;
using UnityEngine;

public class ShrinePanel : MonoBehaviour
{
    public BarIndicator[] ShrineIndicators;
    private Dictionary<Team, BarIndicator> _indicators;
    private float _elapsed = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _indicators = new Dictionary<Team, BarIndicator>();
        _indicators.Add(Team.Chaos, ShrineIndicators[0]);
        _indicators.Add(Team.Balance, ShrineIndicators[1]);
        _indicators.Add(Team.Order, ShrineIndicators[2]);
        Teams.Init();
        foreach (Team key in _indicators.Keys)
        {
            _indicators[key].SetFillColor(Teams.GetTeamColor(key));
        }
        ComponentRegister.ShrinePanel = this;
    }

    // Update is called once per frame
    void Update()
    {
        _elapsed += Time.deltaTime;
        if (_elapsed > 1.0f) 
        {
            _elapsed = 0.0f;
        }
        _indicators[Team.Order].SetFill(_elapsed);
    }
    public void SetFill(Team team, byte health)
    {
        _indicators[team].SetFill(health / 100f);
    }
}
