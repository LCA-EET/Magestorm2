using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Runtime.CompilerServices;
public class MatchEntry : ValidatableForm
{
    public TMP_Text ID, Arena, Creator, TimeLeft, MatchType;
    private ListedMatch _match;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void RefreshTimeRemaining()
    {
        TimeLeft.text = TimeUtil.MinutesAndSecondsRemaining(_match.Expiration);
    }
    public void PopulateFromMatch(ListedMatch match)
    {
        _match = match;
        ID.text = _match.MatchID.ToString();
        Creator.text = _match.CreatorName.ToString();
        Arena.text = LevelData.GetLevel(_match.SceneID).LevelName;
        SetMatchTypeText();
        RefreshTimeRemaining();
        gameObject.SetActive(true);
    }
    public byte MatchID
    {
        get {  return _match.MatchID; }
    }
    public int CreatorAccountID
    {
        get { return _match.CreatorID; }
    }
    public ListedMatch Match
    {
        get { return _match; }
    }
    private void SetMatchTypeText()
    {
        MatchTypes matchType = (MatchTypes)_match.MatchType;
        switch (matchType)
        {
            case MatchTypes.Deathmatch:
                MatchType.text = Language.GetBaseString(103);
                break;
            case MatchTypes.CaptureTheFlag:
                MatchType.text = Language.GetBaseString(105);
                break;
            case MatchTypes.FreeForAll:
                MatchType.text = Language.GetBaseString(104);
                break;
        }
    }
}
