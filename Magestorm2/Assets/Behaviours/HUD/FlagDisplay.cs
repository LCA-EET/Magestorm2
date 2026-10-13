using TMPro;
using UnityEngine;

public class FlagDisplay : MonoBehaviour
{
    public GameObject FlagChaos;
    public GameObject FlagBalance;
    public GameObject FlagOrder;
    public TMP_Text KeyString;
    private PeriodicAction _checkFlag;
    private Team _priorHold = Team.Neutral;
    private void Awake()
    {
        ComponentRegister.FlagDisplay = this;
        if (MatchParams.IncludeFlags)
        {
            _checkFlag = new PeriodicAction(1.0f, CheckFlag, null);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        KeyString.color = Colors.GetTeamColor(MatchParams.MatchTeam);
        RefreshKey();
    }
    public void RefreshKey()
    {
        KeyString.text = InputControls.KeyToString(InputControl.DropFlag);
    }
    private void Update()
    {
        _checkFlag.ProcessAction(Time.deltaTime);
    }
    private void CheckFlag()
    {
        if(FlagManager.FlagHeldByPlayer != _priorHold)
        {
            _priorHold = FlagManager.FlagHeldByPlayer;
            KeyString.gameObject.SetActive(FlagManager.FlagHeldByPlayer != Team.Neutral);
            switch (_priorHold)
            {
                case Team.Chaos:
                    FlagChaos.SetActive(true);
                    break;
                case Team.Balance:
                    FlagBalance.SetActive(true);
                    break;
                case Team.Order:
                    FlagOrder.SetActive(true);
                    break;
                default:
                    HideAll();
                    break;
            }
        }
    }
    
    private void HideAll()
    {
        FlagChaos.SetActive(false);
        FlagBalance.SetActive(false);
        FlagOrder.SetActive(false);
    }
}
