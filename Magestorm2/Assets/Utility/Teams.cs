using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class Teams
{
    private static Dictionary<Team, Sprite> _teamIcons;
    private static Dictionary<Team, int> _teamNames;
    private static Dictionary<Team, Color> _teamColors;
    private static Dictionary<Team, Color> _teamTextColors;
    private static bool _init = false;
    public static void Init()
    {
        if (!_init)
        {
            _teamIcons = new Dictionary<Team, Sprite>();
            _teamIcons.Add(Team.Chaos, Resources.Load<Sprite>("img/team/chaos_colored"));
            _teamIcons.Add(Team.Balance, Resources.Load<Sprite>("img/team/balance_colored"));
            _teamIcons.Add(Team.Order, Resources.Load<Sprite>("img/team/order_colored"));

            _teamColors = new Dictionary<Team, Color>();
            _teamColors.Add(Team.Neutral, Color.white);
            _teamColors.Add(Team.Chaos, Color.red);
            _teamColors.Add(Team.Balance, Color.green);
            _teamColors.Add(Team.Order, Color.blue);
            

            _teamNames = new Dictionary<Team, int>();
            _teamNames.Add(Team.Neutral, 10);
            _teamNames.Add(Team.Chaos, 11);
            _teamNames.Add(Team.Balance, 12);
            _teamNames.Add(Team.Order, 13);
            
            _teamTextColors = new Dictionary<Team, Color>();
            _teamTextColors.Add(Team.Neutral, Color.white);
            _teamTextColors.Add(Team.Chaos, new Color(1f, 0.3451f, 0.3451f, 1f));
            _teamTextColors.Add(Team.Balance, new Color(0, 1f, 0, 1f));
            _teamTextColors.Add(Team.Order, new Color(0.2392f, 0.5176f, 1f, 1f));
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

    public static void SetTextColor(TMP_Text text)
    {
        SetTextColor(text, MatchParams.MatchTeam);
    }

    public static void SetTextColor(TMP_Text text, Team team)
    {
        text.color = _teamTextColors[team];
    }
    public static Color GetTeamTextColor(Team team)
    {
        return _teamTextColors[team];
    }
}
