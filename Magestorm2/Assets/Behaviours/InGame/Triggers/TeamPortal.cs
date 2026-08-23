using UnityEngine;

public class TeamPortal : Portal
{
    public Team Team;
    public GameObject BalancePlane, ChaosPlane, OrderPlane, NeutralPlane;
    private GameObject _portalPlane;
    //public ColoredSprite Sprite;
    private PeriodicAction _checkShrineHealth;
    private bool _shrineIsAlive;

    public void Start()
    {
        //Sprite.Randomize = false;
        //Color toUse = Teams.GetTeamColor(Team);
        //Sprite.SetColor(new Color(toUse.r, toUse.g, toUse.b, 0.5f));
        switch (Team)
        {
            case Team.Chaos:
                _portalPlane = ChaosPlane;
                break;
            case Team.Balance:
                _portalPlane = BalancePlane;
                break;
            case Team.Order:
                _portalPlane = OrderPlane;
                break;
            case Team.Neutral:
                _portalPlane = NeutralPlane;
                break;
        }
        _portalPlane.SetActive(true);
        if (MatchParams.MatchType == ControlCodes.MatchTypes_DeathMatch)
        {
            _checkShrineHealth = new PeriodicAction(1.0f, CheckShrineHealth, null);
        }
    }
    private void CheckShrineHealth()
    {
        _shrineIsAlive = ShrineManager.IsShrineAlive(Team);
        _portalPlane.SetActive(_shrineIsAlive);
    }

    public void Update()
    {
        if (MatchParams.MatchType == ControlCodes.MatchTypes_DeathMatch)
        {
            _checkShrineHealth.ProcessAction(Time.deltaTime);
        }
    }

    public override void EnterAction()
    {
        Debug.Log("Entered " + Teams.GetTeamName(Team) + " portal.");
        if ((_shrineIsAlive && Team == MatchParams.MatchTeam) || (MatchParams.MatchTeam == Team.Neutral))
        {
            base.EnterAction();
        }
    }
}
