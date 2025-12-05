using UnityEngine;
using UnityEngine.UI;

public class TeamChatButton : FormButton
{
    private Image _orb;
    public Team AssociatedTeam;
    private void Awake()
    {
        _orb = GetComponent<Image>();   
        Colors.Init();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        if (AssociatedTeam == Team.Neutral)
        {
            CallBack(buttonType);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void AssignColor(ButtonType selected)
    {
        _orb.color = (selected == buttonType) ? Colors.TeamSelected : Colors.TeamUnselected;
    }
    public override void CallBack(ButtonType selected)
    {
        AssignColor(selected);
        if (selected == buttonType)
        {
            InputField.ChatTarget = AssociatedTeam;
            new MessageData(Language.BuildString(216, Teams.GetTeamName(AssociatedTeam)), "Server", Teams.GetTeamColor(AssociatedTeam));
        }
    }
}
