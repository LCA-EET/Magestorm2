using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public enum Team : byte
{
    Neutral = 0,
    Balance = 1,
    Order = 2,
    Chaos = 3
}
public static class Teams
{
    private static Dictionary<Team, string> _teamNames;
    private static Dictionary<Team, Color> _teamColors;
    private static bool _init = false;
    public static void Init()
    {
        if (!_init)
        {
            _teamColors = new Dictionary<Team, Color>();
            _teamColors.Add(Team.Neutral, Color.white);
            _teamColors.Add(Team.Balance, Color.green);
            _teamColors.Add(Team.Order, Color.blue);
            _teamColors.Add(Team.Chaos, Color.red);

            _teamNames = new Dictionary<Team, string>();
            _teamNames.Add(Team.Neutral, "Neutral");
            _teamNames.Add(Team.Balance, "Balance");
            _teamNames.Add(Team.Order, "Order");
            _teamNames.Add(Team.Chaos, "Chaos");
            _init = true;
        }
    }
    public static Color GetTeamColor(Team team)
    {
        return _teamColors[team];
    }
    public static string GetTeamName(Team team)
    {
        return _teamNames[team];
    }
}
