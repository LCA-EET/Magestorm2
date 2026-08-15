using UnityEngine;
public class TeamDomain : MonoBehaviour
{
    public Team Team;
    
    public void Start()
    {
        TeamTorch[] torches = GetComponentsInChildren<TeamTorch>();
        foreach (TeamTorch torch in torches)
        {
            torch.AssignTeam(Team);
            
        }
        Shrine[] shrines = GetComponentsInChildren<Shrine>();
        foreach(Shrine shrine in shrines)
        {
            shrine.AssignToTeam(Team) ;
        }
        Flag[] flags = GetComponentsInChildren<Flag>();
        foreach(Flag flag in flags)
        {
            flag.AssignToTeam(Team);
        }
    }
}
