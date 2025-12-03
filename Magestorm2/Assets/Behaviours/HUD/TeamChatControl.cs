using UnityEngine;

public class TeamChatControl : ValidatableForm
{
    private Team _selectedTeam;
    private void Awake()
    {
        if (!MatchParams.IncludeTeams)
        {
            Destroy(gameObject);
        }
        else
        {
            ComponentRegister.TeamChatControl = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ButtonPressed(ButtonType buttonType)
    {
        _buttonTable[buttonType].CallBack(buttonType);
    }
    public void SetTeam(Team team)
    {
        _selectedTeam = team;
    }
    public Team SelectedTeam
    {
        get { return _selectedTeam; }
    }
}
