using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class ListedMatch
{
    private byte _matchID, _sceneID;
    private long _expiration;
    private String _creatorName;
    public ListedMatch(byte matchID, byte sceneID, string creatorName, long expiration)
    {
        _matchID = matchID;
        _sceneID = sceneID;
        _creatorName = creatorName;
        _expiration = expiration;
    }

    public byte MatchID
    {
        get { return _matchID; }
    }
    public byte SceneID
    {
        get { return _sceneID; }
    }
    public long Expiration
    {
        get { return _expiration; }
    }
    public String CreatorName
    {
        get { return _creatorName; }
    }
}

