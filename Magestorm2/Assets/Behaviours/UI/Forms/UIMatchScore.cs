using System;
using UnityEngine;
public class UIMatchScore : ValidatableForm
{
    public PlayerScoreEntry[] PlayerScoreEntries;

    public void PopulateForm(byte[] data, bool resetMatchID)
    {
        int index = 1;
        byte numPlayers = data[index];
        index++;
        byte pseIndex = 0;
        for (byte b = 0; b < numPlayers; b++)
        {
            byte kills = data[index];
            index++;
            byte deaths = data[index];
            index++;
            byte raises = data[index];
            index++;
            byte level = data[index];
            index++;
            byte classCode = data[index];
            index++;
            byte nameLength = data[index];
            index++;
            string playerName = ByteUtils.BytesToUTF8(data, index, nameLength);
            index += nameLength;
            PlayerScoreEntry pse = PlayerScoreEntries[pseIndex];
            pse.PlayerName.text = playerName;
            pse.Level.text = level.ToString();
            pse.Class.text = CharacterClassManager.GetCharacterClassData(classCode).Abbreviation;
            pse.Kills.text = kills.ToString();
            pse.Deaths.text = deaths.ToString();
            pse.Raises.text = raises.ToString();
            pse.TotalScore.text = DetermineScore(kills, deaths, raises).ToString();
            pse.gameObject.SetActive(true);
            pseIndex++;
            pse.Position.text = pseIndex.ToString();
        }
        while(pseIndex < PlayerScoreEntries.Length)
        {
            Debug.Log("Disabling pse " + pseIndex);
            PlayerScoreEntries[pseIndex].gameObject.SetActive(false);
            pseIndex++;
        }
        if (resetMatchID)
        {
            MatchParams.MatchID = 0;
        }
    }
    private short DetermineScore(byte kills, byte deaths, byte raises)
    {
        return (short)((kills - deaths) + Mathf.Floor(0.5f * raises));
    }
    void Start()
    {
        AssociateFormToButtons();
    }
}
