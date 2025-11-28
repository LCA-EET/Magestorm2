using UnityEngine;

public class TeamTorch : MonoBehaviour
{
    public Team Team;
    private Torchelight _torchelight;
    public void Awake()
    {
        _torchelight = GetComponent<Torchelight>();
    }
    public void Update()
    {
     
    }
}
