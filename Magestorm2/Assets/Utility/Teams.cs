using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public static class Teams
{
    public static Sprite IconBalance, IconChaos, IconOrder;
    private static Dictionary<Team, Sprite> _teamIcons;
    private static Dictionary<Team, int> _teamNames;
    private static Dictionary<Team, Color> _teamColors;
    private static bool _init = false;
    public static void Init()
    {
        if (!_init)
        {
            _teamIcons = new Dictionary<Team, Sprite>();
            _teamIcons.Add(Team.Balance, IconBalance);
            _teamIcons.Add(Team.Chaos, IconChaos);
            _teamIcons.Add(Team.Order, IconOrder);

            _teamColors = new Dictionary<Team, Color>();
            _teamColors.Add(Team.Neutral, Color.white);
            _teamColors.Add(Team.Balance, Color.green);
            _teamColors.Add(Team.Order, Color.blue);
            _teamColors.Add(Team.Chaos, Color.red);

            _teamNames = new Dictionary<Team, int>();
            _teamNames.Add(Team.Neutral, 9);
            _teamNames.Add(Team.Balance, 11);
            _teamNames.Add(Team.Order, 12);
            _teamNames.Add(Team.Chaos, 10);
            _init = true;
        }
    }
    public static Sprite GetTeamIcon(Team team)
    {
        return _teamIcons[team];
    }
    public static Color GetTeamColor(Team team)
    {
        return _teamColors[team];
    }
    public static string GetTeamName(Team team)
    {
        return Language.GetBaseString(_teamNames[team]);
    }
}
