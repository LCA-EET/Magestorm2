using UnityEngine;
using UnityEngine.UI;

public class TeamPlayerList : MonoBehaviour
{
    public Team TeamID;
    private Image _teamIcon;
    public PlayerEntry[] PlayerEntries;
    private void Awake()
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _teamIcon = GetComponentInChildren<Image>();
       // _teamIcon.color = Teams.GetTeamColor(TeamID);
        _teamIcon.sprite = Teams.GetTeamIcon(TeamID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FillTeam(RemotePlayerData[] teamPlayers)
    {
        int i = 0;
        for (i = 0; i < teamPlayers.Length; i++)
        {
            RemotePlayerData teamPlayer = teamPlayers[i];
            if(i < PlayerEntries.Length)
            {
                PlayerEntries[i].SetText(teamPlayer.Name + " " + teamPlayer.Level + " " + SharedFunctions.ClassAbbreviation(teamPlayer.PlayerClass));
                PlayerEntries[i].gameObject.SetActive(true);
            }
        }
        while (i < PlayerEntries.Length)
        {
            PlayerEntries[i].gameObject.SetActive(false);
            i = i + 1;
        }
    }
}
