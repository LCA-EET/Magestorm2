using UnityEngine;
using UnityEngine.UI;

public class TeamPlayerList : MonoBehaviour
{
    public Team TeamID;
    private Image _teamIcon;
    private void Awake()
    {
        _teamIcon = GetComponent<Image>();
        _teamIcon.color = Teams.GetTeamColor(TeamID);
        _teamIcon.sprite = Teams.GetTeamIcon(TeamID);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
