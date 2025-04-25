using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

public class UIMatchList : ValidatableForm
{
    public MatchEntry[] MatchEntries;
    public SelectionGroup MatchSelectionGroup;
    private float _elapsed = 0.0f;
    private void Awake()
    {
        Game.SendBytes(Packets.SubscribeToMatchesPacket());
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        AssociateFormToButtons();
        ClearEntries(0);
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        UIAudio.PlayButtonPress();
        switch (buttonType)
        {
            case ButtonType.CharacterSelect:
                Game.SendBytes(Packets.UnsubscribeFromMatchesPacket());
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
        }
    }
    private void JoinMatch()
    {
        MatchEntry selected = GetSelectedEntry();
        if(selected == null)
        {
            Game.MessageBox(Language.GetBaseString(50));
        }
        else
        {

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
            Game.MessageBox(Language.GetBaseString(45));
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
            Game.MessageBox(Language.GetBaseString(50));
        }
        else
        {
            if (selected.CreatorAccountID == PlayerAccount.AccountID)
            {
                Game.SendBytes(Packets.DeleteMatchPacket());
            }
            else
            {
                Game.MessageBox(Language.GetBaseString(51));
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        _elapsed += Time.deltaTime;
        if (_elapsed > 1.0f)
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
        }
    }
    private void ClearEntries(int index)
    {
        for(int i = index; i < MatchEntries.Length; i++)
        {
            MatchEntries[i].gameObject.SetActive(false);
        }
    }
}
