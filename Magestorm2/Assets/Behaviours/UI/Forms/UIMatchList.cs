using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class UIMatchList : ValidatableForm
{
    public MatchEntry[] MatchEntries;
    public SelectionGroup MatchSelectionGroup;
    public TMP_Text NoMatchesText;
    private float _elapsed = 0.0f;
    private void Awake()
    {
        Game.SendPregameBytes(Pregame_Packets.RequestMatchListPacket());
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        AssociateFormToButtons();
        ClearEntries(0);
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.CharacterSelect:
                Game.SendPregameBytes(Pregame_Packets.UnsubscribeFromMatchesPacket());
                ComponentRegister.UIPrefabManager.PopFromStack();
                ActiveMatches.ClearMatches();
                break;
            case ButtonType.CreateMatch:
                CreateMatch();
                break;
            case ButtonType.DeleteMatch:
                DeleteMatch();
                break;
            case ButtonType.JoinMatch:
                JoinMatch();
                break;
            case ButtonType.Misc0: // Discord
                System.Diagnostics.Process.Start("https://discord.com/invite/hwGf39gW9g");
                break;
        }
    }
    private void JoinMatch()
    {
        MatchEntry selected = GetSelectedEntry();   
        if(selected == null)
        {
            Game.MessageBox(Language.GetBaseString(51)); //
        }
        else
        {
            if(selected.GetMatchType() == (byte)MatchTypes.FreeForAll)
            {
                MatchParams.ExpirationTime = selected.Match.Expiration;
                Game.SendPregameBytes(Pregame_Packets.JoinMatchPacket(selected.MatchID, 0));
            }
            else
            {
                Debug.Log("Requesting match details: " + selected.MatchID);
                Game.SendPregameBytes(Pregame_Packets.MatchDetailsPacket(selected.MatchID));
            }
        }
    }
    private void CreateMatch()
    {
        bool matchAlreadyCreated = false;
        foreach (MatchEntry entry in MatchEntries)
        {
            if (entry.gameObject.activeSelf)
            {
                if (entry.CreatorAccountID == PlayerAccount.AccountID)
                {
                    matchAlreadyCreated = true;
                    break;
                }
            }
        }
        if (matchAlreadyCreated)
        {
            Game.MessageBox(Language.GetBaseString(46)); //
        }
        else
        {
            ComponentRegister.UIPrefabManager.InstantiateMatchCreator();
        }
    }
    private MatchEntry GetSelectedEntry()
    {
        MatchEntry toReturn = null;
        int selectedIndex = MatchSelectionGroup.SelectedIndex;
        if(selectedIndex != -1)
        {
            toReturn = MatchEntries[selectedIndex];
        }
        return toReturn;
    }
    private void DeleteMatch()
    {
        MatchEntry selected = GetSelectedEntry();
        if(selected == null)
        {
            Game.MessageBox(Language.GetBaseString(51)); //
        }
        else
        {
            if (selected.CreatorAccountID == PlayerAccount.AccountID)
            {
                Game.SendPregameBytes(Pregame_Packets.DeleteMatchPacket());
            }
            else
            {
                Game.MessageBox(Language.GetBaseString(52)); //
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        _elapsed += Time.deltaTime;
        if (_elapsed > 0.5f)
        {
            _elapsed = 0.0f;
            if (ActiveMatches.UpdatesMade)
            {
                List<ListedMatch> matches = ActiveMatches.MatchListing();
                int index = 0;
                foreach (ListedMatch match in matches)
                {
                    MatchEntries[index].PopulateFromMatch(match);
                    index++;
                }
                ClearEntries(index);
                MatchSelectionGroup.DeselectAll();
            }
            else
            {
                foreach(MatchEntry entry in MatchEntries)
                {
                    if (entry.isActiveAndEnabled)
                    {
                        entry.RefreshTimeRemaining();
                    }
                }
            }
            if (LevelData.LevelCount == 0)
            {
                ComponentRegister.PregamePacketProcessor.SendBytes(Pregame_Packets.RequestLevelsListPacket());
            }
        }
    }
    private void ClearEntries(int index)
    {
        for(int i = index; i < MatchEntries.Length; i++)
        {
            MatchEntries[i].gameObject.SetActive(false);
        }
        NoMatchesText.gameObject.SetActive(index == 0);
    }
}
