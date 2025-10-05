using UnityEngine;

public class BiasIndicator : MonoBehaviour
{
    public MeshRenderer PlaneRenderer;

    public void Start()
    {
        
    }
    public void ChangeBias(Team team)
    {
        switch (team)
        {
            case Team.Balance:
                PlaneRenderer.material = ComponentRegister.SceneInitializer.BalanceBiased;
                break;
            case Team.Order:
                PlaneRenderer.material = ComponentRegister.SceneInitializer.OrderBiased;
                break;
            case Team.Chaos:
                PlaneRenderer.material = ComponentRegister.SceneInitializer.ChaosBiased;
                break;
            case Team.Neutral:
                PlaneRenderer.material = ComponentRegister.SceneInitializer.NeutralBiased;
                break;
        }
    }
}

