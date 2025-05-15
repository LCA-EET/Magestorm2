using UnityEngine;
using UnityEngine.UI;

public class TeamPlayerList : MonoBehaviour
{
    public Team TeamID;
    private Image _teamIcon;
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
}
