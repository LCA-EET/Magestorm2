using UnityEngine;

public class BiasIndicator : MonoBehaviour
{
    public SpriteRenderer Renderer;

    public void Start()
    {
        
    }
    public void ChangeBias(Team team)
    {
        Renderer.color = Colors.GetTeamColor(team);
    }
}

