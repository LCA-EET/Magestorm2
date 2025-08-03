using TMPro;
using UnityEngine;

public class UIJoinMatch : ValidatableForm
{
    public TeamPlayerList ChaosPlayerList;
    public TeamPlayerList BalancePlayerList;
    public TeamPlayerList OrderPlayerList;
    public TMP_Text MatchCreatorText;
    public TMP_Text MatchIDText;
    public TMP_Text MatchLevelText;
    private Team _selectedTeam;
    private ListedMatch _match;

    private void Awake()
    {
        _selectedTeam = Team.Neutral;
        ComponentRegister.UIJoinMatch = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
        object[] matchParams = SharedFunctions.Params;
        _match = (ListedMatch)matchParams[0];
        ChaosPlayerList.FillTeam((RemotePlayerData[])matchParams[2]);
        BalancePlayerList.FillTeam((RemotePlayerData[])matchParams[1]);
        OrderPlayerList.FillTeam((RemotePlayerData[])matchParams[3]);

        MatchIDText.text = Language.BuildString(97,_match.MatchID.ToString());
        MatchLevelText.text = Language.BuildString(99,_match.SceneName);
        MatchCreatorText.text = Language.BuildString(98, _match.CreatorName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void JoinMatch()
    {
        if (ValidateForm())
        {
            MatchParams.ExpirationTime = _match.Expiration;
            Game.SendBytes(Pregame_Packets.JoinMatchPacket(_match.MatchID, (byte)_selectedTeam));
        }
        else
        {
            Game.MessageBoxReference(82);
        }
    }
    protected override bool ValidateForm()
    {
        return _selectedTeam > Team.Neutral;
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
                JoinMatch();
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
