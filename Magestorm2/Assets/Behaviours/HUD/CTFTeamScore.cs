using TMPro;
using UnityEngine;

public class CTFTeamScore : MonoBehaviour
{
    public TMP_Text Score;
    public TMP_Text TeamName;
    public Team Team;

    public void Refresh()
    {
        TeamName.text = Teams.GetTeamName(Team);
        Score.text = FlagManager.GetScore(Team).ToString();
    }
}
