using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

public class UIMatchList : ValidatableForm
{
    public MatchEntry[] MatchEntries;
    private float _elapsed = 0.0f;
    private void Awake()
    {
        Game.SendBytes(Packets.SubscribeToMatchesPacket());
        foreach (MatchEntry entry in MatchEntries)
        {
            entry.SetOwningList(this);
        }
    }
    public void EntrySelected(MatchEntry selectedEntry)
    {
        foreach (MatchEntry entry in MatchEntries)
        {
            entry.MarkSelected(false);
        }
        selectedEntry.MarkSelected(true);
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
                Game.SendBytes(Packets.CreateMatchPacket(0));
                break;
            case ButtonType.DeleteMatch:
                DeleteMatch();
                break;
        }
    }
    private void DeleteMatch()
    {
        MatchEntry selected = SelectedEntry();
        if (selected != null)
        {
            if(selected.CreatorAccountID == PlayerAccount.AccountID)
            {
                Game.SendBytes(Packets.DeleteMatchPacket());
            }
            else
            {
                Game.MessageBox(Language.GetBaseString(51));
            }
        }
        else
        {
            Game.MessageBox(Language.GetBaseString(50));
        }
    }
    public MatchEntry SelectedEntry()
    {
        foreach(MatchEntry entry in MatchEntries)
        {
            if (entry.IsSelected)
            {
                return entry;
            }
        }
        return null;
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
