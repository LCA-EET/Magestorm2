using UnityEngine;

public class TeamPortal : Portal
{
    public Team Team;
    public GameObject PortalPlane;
    public ColoredSprite Sprite;
    private PeriodicAction _checkShrineHealth;
    private bool _shrineIsAlive;
    public void Awake()
    {
        Sprite.Randomize = false;
        Sprite.SetColor(Teams.GetTeamColor(Team));
        if(MatchParams.MatchType == (byte)MatchTypes.Deathmatch)
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
        if (_shrineIsAlive && Team == MatchParams.MatchTeam)
        {
            base.EnterAction();
        }
    }
}
