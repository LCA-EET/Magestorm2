using UnityEngine;

public class TeamTorch : MonoBehaviour
{
    public Light Light;
    private ParticleSystem[] _particleSystems;
    private Torchelight _torchelight;
    private Team _team;
    private int[] _initialParticleCounts;
    public GameObject Balance, Chaos, Order, Neutral;
    public void Awake()
    {
        _torchelight = GetComponent<Torchelight>();
        SetEmissions(Neutral);
    }
    private void SetEmissions(GameObject parentObject)
    {
        parentObject.SetActive(true);
        _particleSystems = parentObject.GetComponentsInChildren<ParticleSystem>();
        _initialParticleCounts = new int[_particleSystems.Length];
        for (int i = 0; i < _particleSystems.Length; i++)
        {
            _initialParticleCounts[i] = _particleSystems[0].main.maxParticles;
            _torchelight.SetComponentEmissions(_particleSystems[i], i);
        }
    }
    public Team Team
    {
        get
        {
            return _team;
        }
    }
    public void AssignTeam(Team team)
    {
        _team = team;
        GameObject selected = null;
        switch (team)
        {
            case Team.Balance:
                selected = Balance;
                break;
            case Team.Chaos:
                selected = Chaos;
                break;
            case Team.Order:
                selected = Order;
                break;
        }
        SetEmissions(selected);
        Neutral.SetActive(false);
        if (MatchParams.MatchType == ControlCodes.MatchTypes_DeathMatch && Team != Team.Neutral)
        {
            TorchManager.RegisterTorch(this);
        }
        Light.color = Colors.GetTeamColor(Team);
    }
    public void Start()
    {
        
        
    }

    public void SetIntensity (float intensity)
    {
        _torchelight.IntensityLight = intensity * 3.0f;
        for(int i = 0; i < _particleSystems.Length; i++)
        {
            ParticleSystem.MainModule pMain = _particleSystems[i].main;
            pMain.maxParticles = Mathf.RoundToInt(_initialParticleCounts[i] * intensity);
        }
    }
}
