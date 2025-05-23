using UnityEngine;

public class UIJoinMatch : ValidatableForm
{
    public TeamPlayerList ChaosPlayerList;
    public TeamPlayerList BalancePlayerList;
    public TeamPlayerList OrderPlayerList;

    private Team _selectedTeam;

    private void Awake()
    {
        _selectedTeam = Team.Neutral;
        ComponentRegister.UIJoinMatch = this;
        Game.SendBytes(Packets.MatchDetailsPacket((MatchEntry)SharedFunctions.Params[0]));
        
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

    public void FillPlayers(RemotePlayerData[] chaos, RemotePlayerData[] balance, RemotePlayerData[] order)
    {
        ChaosPlayerList.FillTeam(chaos);
        BalancePlayerList.FillTeam(balance);
        OrderPlayerList.FillTeam(order);
    }
    protected override bool ValidateForm()
    {
        if(_selectedTeam == Team.Neutral)
        {
            Game.MessageBoxReference(82);
        }
        else
        {

        }
        return base.ValidateForm();
    }
    private void SelectTeam(Team team)
    {
        ChaosPlayerList.MarkSelected(team);
        BalancePlayerList.MarkSelected(team);
        OrderPlayerList.MarkSelected(team);
        _selectedTeam = team;
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Cancel:
                CloseForm();
                break;
            case ButtonType.Submit:
                ValidateForm();
                break;
            case ButtonType.Misc0:
                SelectTeam(Team.Chaos);
                break;
            case ButtonType.Misc1:
                SelectTeam(Team.Balance);
                break;
            case ButtonType.Misc2:
                SelectTeam(Team.Order);
                break;
        }
    }
}
