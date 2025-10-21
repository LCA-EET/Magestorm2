using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatPanel : MonoBehaviour
{
    public TMP_Text TotalText;
    private Dictionary<PlayerStats, StatLine> _statTable;
    private void Awake()
    {
        _statTable = new Dictionary<PlayerStats, StatLine>();
        StatLine[] statLines = GetComponentsInChildren<StatLine>();
        foreach (StatLine statLine in statLines)
        {
            _statTable.Add(statLine.Statistic, statLine);
            statLine.AssignOwner(this);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RefreshTotal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RefreshTotal()
    {
        TotalText.text = Language.BuildString(69, StatTotal()); //
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
}
