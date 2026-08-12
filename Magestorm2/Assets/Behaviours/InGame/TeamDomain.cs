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
    }
}
