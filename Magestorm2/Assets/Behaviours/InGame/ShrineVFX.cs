using UnityEngine;
public class ShrineVFX : MonoBehaviour
{
    public ParticleSystem EmissionModuleA;
    public ParticleSystem EmissionModuleB;
    public Light A, B;
    private ParticleSystem.EmissionModule _mmA, _mmB;
    private float _emissionsRate;
    private bool _initialized;

    private void Awake()
    {
        Initialize();
        GameSettings.ApplyLightShadowSetting(A);
        GameSettings.ApplyLightShadowSetting(B);
    }

    private void Initialize()
    {
        _emissionsRate = EmissionModuleA.emission.rateOverTime.constantMax;
        _mmA = EmissionModuleA.emission;
        _mmB = EmissionModuleB.emission;
        _initialized = true;
    }
    public void UpdateRate(float health)
    {
        if(!_initialized)
        {
            Initialize();
        }
        _mmA.rateOverTime = _emissionsRate * health;
        _mmB.rateOverTime = _emissionsRate * health;
        A.intensity = 2.0f * health;
        B.intensity = 2.0f * health;
    }
}
