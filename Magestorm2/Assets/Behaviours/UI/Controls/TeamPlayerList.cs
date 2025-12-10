using UnityEngine;
using UnityEngine.UI;

public class TeamPlayerList : MonoBehaviour
{
    public Team TeamID;
    public Image _teamIcon;
    public PlayerEntry[] PlayerEntries;
    public GameObject NoPlayersHeader;
    private Color _initial;
    private int _startIndex;
    private int _teamPlayerCount;
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
    public void DecrementStartIndex()
    {
        if(_startIndex > 0)
        {
            _startIndex--;
        }
    }
    public void IncrementStartIndex()
    {
        if((_startIndex * 10) < _teamPlayerCount)
        {
            _startIndex++;
        }
    }
    public void FillTeam(RemotePlayerData[] teamPlayers)
    {
        int i = _startIndex * 10;
        _teamPlayerCount = teamPlayers.Length;
        while (i < teamPlayers.Length)
        {
            RemotePlayerData teamPlayer = teamPlayers[i];
            PlayerEntry entry;
            if (i < PlayerEntries.Length)
            {
                entry = PlayerEntries[i];
                entry.SetText(teamPlayer.Name + " " + teamPlayer.Level + " " + SharedFunctions.ClassAbbreviation(teamPlayer.PlayerClass));
                entry.gameObject.SetActive(true);
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
