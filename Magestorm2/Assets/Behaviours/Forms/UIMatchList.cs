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
        switch (buttonType)
        {
            case ButtonType.CharacterSelect:
                Game.SendBytes(Packets.UnsubscribeFromMatchesPacket());
                ComponentRegister.UIPrefabManager.PopFromStack();
                break;
            case ButtonType.CreateMatch:
                Game.SendBytes(Packets.CreateMatchPacket(0));
                break;
            case ButtonType.DeleteMatch:
                Game.SendBytes(Packets.DeleteMatchPacket());
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _elapsed += Time.deltaTime;
        if (_elapsed > 1.0f)
        {
            _elapsed = 0.0f;
            if (ActiveMatches.MatchCount > 0)
            {
                List<ListedMatch> matches = new List<ListedMatch>();
                int index = 0;
                foreach (ListedMatch match in matches)
                {
                    MatchEntries[index].PopulateFromMatch(match);
                    index++;
                }
                ClearEntries(index);
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
