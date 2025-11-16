using System.Collections.Generic;
using UnityEngine;

public class CTFScorePanel : MonoBehaviour
{
    public CTFTeamScore ChaosEntry;
    public CTFTeamScore BalanceEntry;
    public CTFTeamScore OrderEntry;

    private Dictionary<Team, CTFTeamScore> _teamScores;
    private void Awake()
    {
        _teamScores = new Dictionary<Team, CTFTeamScore>();
        if (!MatchParams.IncludeFlags)
        {
            Destroy(gameObject);
        }
        else
        {
            ComponentRegister.CTFScorePanel = this;
            _teamScores.Add(Team.Chaos, ChaosEntry);
            _teamScores.Add(Team.Balance, BalanceEntry);
            _teamScores.Add(Team.Order, OrderEntry);
        }
    }
    private void Start()
    {
        RefreshScores();
    }
    public void RefreshScores()
    {
        foreach (CTFTeamScore score in _teamScores.Values)
        {
            score.Refresh();
        }
    }
}
