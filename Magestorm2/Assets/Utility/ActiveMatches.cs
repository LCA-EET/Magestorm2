using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ActiveMatches
{
    private static Dictionary<byte, ListedMatch> _activeMatches;
    public static bool UpdatesMade;
    private static bool _init = false;

    public static void Init()
    {
        if (!_init)
        {
            _activeMatches = new Dictionary<byte, ListedMatch>();
            _init = true;
        }
    }

    public static List<ListedMatch> MatchListing()
    {
        UpdatesMade = false;
        return _activeMatches.Values.ToList<ListedMatch>();
    }
    public static void ClearMatches()
    {
        _activeMatches.Clear();
        UpdatesMade = true;
    }
    public static byte MatchCount
    {
        get
        {
            return (byte) _activeMatches.Count;
        }
    }

    public static void AddMatch(ListedMatch match)
    {
        //Debug.Log("Match added.");
        _activeMatches.Add(match.MatchID, match);
        UpdatesMade = true;
    }

    public static bool GetMatch(byte matchID, ref ListedMatch match)
    {
        if (_activeMatches.ContainsKey(matchID))
        {
            match = _activeMatches[matchID];
            return true;
        }
        return false;
    }
}
