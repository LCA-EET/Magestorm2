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
        Flag[] flags = GetComponentsInChildren<Flag>();
        if (MatchParams.IncludeShrines)
        {
            foreach (Shrine shrine in shrines)
            {
                shrine.AssignToTeam(Team);
            }
        }
        else
        {
            foreach (Shrine shrine in shrines)
            {
                shrine.gameObject.SetActive(false);
            }
        }
        if (MatchParams.IncludeFlags)
        {
            foreach (Flag flag in flags)
            {
                flag.AssignToTeam(Team);
            }
        }
        else
        {
            foreach (Flag flag in flags)
            {
                flag.gameObject.SetActive(false);
            }
        }
    }
}
