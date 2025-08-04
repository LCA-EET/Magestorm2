using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class ListedMatch
{
    private byte _matchID, _sceneID, _matchType;
    private long _expiration;
    private int _creatorID;
    private string _sceneName;
    private String _creatorName;
    public ListedMatch(byte matchID, byte sceneID, string creatorName, long expiration, int creatorID, byte matchType)
    {
        _matchType = matchType;
        _matchID = matchID;
        _sceneID = sceneID;
        _creatorName = creatorName;
        _expiration = expiration;
        _creatorID = creatorID;
        _sceneName = LevelData.GetLevel(sceneID).LevelName;
    }

    public string MatchTypeString
    {
        get
        {
            MatchTypes matchType = (MatchTypes)MatchType;
            switch (matchType)
            {
                case MatchTypes.Deathmatch:
                    return Language.GetBaseString(103);
                case MatchTypes.CaptureTheFlag:
                    return Language.GetBaseString(105);
                case MatchTypes.FreeForAll:
                    return Language.GetBaseString(104);
            }
            return "";
        }
    }
    public int CreatorID
    {
        get { return _creatorID; }
    }
    public byte MatchID
    {
        get { return _matchID; }
    }
    public byte MatchType
    {
        get { return _matchType; }
    }
    public byte SceneID
    {
        get { return _sceneID; }
    }
    public long Expiration
    {
        get { return _expiration; }
    }
    public string CreatorName
    {
        get { return _creatorName; }
    }
    public string SceneName
    {
        get { return _sceneName; }
    }
}

