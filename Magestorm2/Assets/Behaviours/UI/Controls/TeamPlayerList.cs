using UnityEngine;
using UnityEngine.UI;

public class TeamPlayerList : MonoBehaviour
{
    public Team TeamID;
    public Image _teamIcon;
    public PlayerEntry[] PlayerEntries;
    public GameObject NoPlayersHeader;
    private Color _initial;
    private void Awake()
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _initial = _teamIcon.color;
        _teamIcon.sprite = Teams.GetTeamIcon(TeamID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MarkSelected(Team team)
    {
        _teamIcon.color = (TeamID == team) ? Teams.GetTeamColor(TeamID) : _initial;
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
        NoPlayersHeader.gameObject.SetActive(teamPlayers.Length == 0);
    }
}
