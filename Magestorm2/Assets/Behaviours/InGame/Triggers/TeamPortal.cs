using UnityEngine;

public class TeamPortal : Portal
{
    public Team Team;
    public GameObject PortalPlane;
    public ColoredSprite Sprite;
    private PeriodicAction _checkShrineHealth;
    private bool _shrineIsAlive;

    public void Start()
    {
        Sprite.Randomize = false;
        Color toUse = Teams.GetTeamColor(Team);
        Sprite.SetColor(new Color(toUse.r, toUse.g, toUse.b, 0.5f));
        if (MatchParams.MatchType == (byte)MatchTypes.Deathmatch)
        {
            _checkShrineHealth = new PeriodicAction(1.0f, CheckShrineHealth, null);
        }
    }
    private void CheckShrineHealth()
    {
        _shrineIsAlive = ShrineManager.IsShrineAlive(Team);
        PortalPlane.SetActive(_shrineIsAlive);
    }

    public void Update()
    {
        if (MatchParams.MatchType == (byte)MatchTypes.Deathmatch)
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
