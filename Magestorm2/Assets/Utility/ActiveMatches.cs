using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ActiveMatches
{
    private static Dictionary<int, ListedMatch> _activeMatches;

    public static void Init()
    {
        _activeMatches = new Dictionary<int, ListedMatch>();
    }

    public static List<ListedMatch> MatchListing()
    {
        return _activeMatches.Values.ToList<ListedMatch>();
    }

    public static byte MatchCount
    {
        get
        {
            return (byte) _activeMatches.Count;
        }
    }
}
