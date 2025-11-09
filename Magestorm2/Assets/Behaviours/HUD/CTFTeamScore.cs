using TMPro;
using UnityEngine;

public class CTFTeamScore : MonoBehaviour
{
    public TMP_Text Score;
    public TMP_Text TeamName;
    public Team Team;
    public void UpdateScore(byte newScore)
    {
        Score.text = newScore.ToString();
    }
    public void Refresh()
    {
        Score.text = TeamName.text + ": " + FlagManager.GetScore(Team);
    }
}
