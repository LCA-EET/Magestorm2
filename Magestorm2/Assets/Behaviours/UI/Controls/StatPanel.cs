using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatPanel : ValidateableObject
{
    public TMP_Text TotalText;
    private Dictionary<PlayerStats, StatLine> _statTable;
    private bool _readOnly = false;
    private StatLine[] _statLines;
    
    private void Awake()
    {
        _statTable = new Dictionary<PlayerStats, StatLine>();
        _statLines = GetComponentsInChildren<StatLine>();
        foreach (StatLine statLine in _statLines)
        {
            _statTable.Add(statLine.Statistic, statLine);
            statLine.AssignOwner(this);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!_readOnly)
        {
            RefreshTotal();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MakeReadOnly()
    {
        _readOnly = true;
        foreach (StatLine statLine in _statLines)
        {
            statLine.DisableButtons();
        }
    }
    public void RefreshTotal()
    {
        TotalText.text = Language.BuildString(69, StatTotal()); //
    }
    public void FillStats(PlayerCharacter character)
    {
        byte[] stats = character.StatBytes;
        for (byte b = 0; b < stats.Length; b++)
        {
            FillStat((PlayerStats)b, stats[b]);
        }
    }
    public void DisablePanel()
    {
        foreach (StatLine statLine in _statLines)
        {
            statLine.DisableButtons();
        }
    }
    public void FillStat(PlayerStats stat, byte value)
    {
        _statTable[stat].Value = value;
    }
    public byte StatTotal()
    {
        byte total = 0;
        foreach (StatLine line in _statTable.Values)
        {
            total += line.Value;
        }
        return total;
    }
    public byte[] GetStats()
    {
        byte[] toReturn = new byte[6];
        toReturn[0] = _statTable[PlayerStats.Strength].Value;
        toReturn[1] = _statTable[PlayerStats.Dexterity].Value;
        toReturn[2] = _statTable[PlayerStats.Constitution].Value;
        toReturn[3] = _statTable[PlayerStats.Intellect].Value;
        toReturn[4] = _statTable[PlayerStats.Charisma].Value;
        toReturn[5] = _statTable[PlayerStats.Wisdom].Value;
        return toReturn;
    }

    public override bool Validate()
    {
        bool toReturn = StatTotal() == 90;
        if (!toReturn)
        {
            _validationFailureMessage = Language.GetBaseString(290);
        }
        else
        {
            _validationFailureMessage = "";
        }
        return StatTotal() == 90;
    }

    public override void MarkInvalid(bool invalid)
    {
        Game.MessageBoxReference(290);
    }
}
