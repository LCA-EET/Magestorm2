using UnityEngine;

public class TeamTorch : MonoBehaviour
{
    public Team Team;
    public Light Light;
    public ParticleSystem[] ParticleSystems;
    private Torchelight _torchelight;
    private int[] _initialParticleCounts;
    public void Awake()
    {
        _torchelight = GetComponent<Torchelight>();
        _initialParticleCounts = new int[ParticleSystems.Length];
        for(int i = 0; i < ParticleSystems.Length; i++)
        {
            _initialParticleCounts[i] = ParticleSystems[0].main.maxParticles;
        }
    }
    public void Start()
    {
        if(MatchParams.MatchType == (byte)MatchTypes.Deathmatch)
        {
            TorchManager.RegisterTorch(this);
        }
        Light.color = Colors.GetTeamColor(Team);
    }

    public void SetIntensity (float intensity)
    {
        _torchelight.IntensityLight = intensity * 3.0f;
        for(int i = 0; i < ParticleSystems.Length; i++)
        {
            ParticleSystem.MainModule pMain = ParticleSystems[i].main;
            pMain.maxParticles = Mathf.RoundToInt(_initialParticleCounts[i] * intensity);
        }
    }
}
