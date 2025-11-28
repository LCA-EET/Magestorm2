using System.Collections.Generic;

public static class TorchManager
{
    private static Dictionary<Team, HashSet<TeamTorch>> _teamTorches;
    public static void Init()
    {
        _teamTorches = new Dictionary<Team, HashSet<TeamTorch>>();
        _teamTorches.Add(Team.Neutral, new HashSet<TeamTorch>());
        _teamTorches.Add(Team.Chaos, new HashSet<TeamTorch>());
        _teamTorches.Add(Team.Balance, new HashSet<TeamTorch>());
        _teamTorches.Add(Team.Order, new HashSet<TeamTorch>());
    }
    public static void RegisterTorch(TeamTorch toRegister)
    {
        _teamTorches[toRegister.Team].Add(toRegister);
    }
    public static void AdjustTeamTorchIntensity(Team team, float intensity)
    {
        HashSet<TeamTorch> torches = _teamTorches[team];
        foreach (TeamTorch torch in torches)
        {
            torch.SetIntensity(intensity);
        }
    }
}
