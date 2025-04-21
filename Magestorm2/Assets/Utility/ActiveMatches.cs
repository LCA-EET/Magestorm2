using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ActiveMatches
{
    private static Dictionary<int, ListedMatch> _activeMatches;
    public static bool UpdatesMade;

    public static void Init()
    {
        _activeMatches = new Dictionary<int, ListedMatch>();
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
        Debug.Log("Match added.");
        _activeMatches.Add(match.MatchID, match);
        UpdatesMade = true;
    }

}
