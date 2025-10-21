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
    public void ChangeScore(Team team, byte newScore)
    {
        _teamScores[team].UpdateScore(newScore);
    }
}
