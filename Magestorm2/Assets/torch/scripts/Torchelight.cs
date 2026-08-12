using UnityEngine;

public class Torchelight : MonoBehaviour {
	
	public GameObject TorchLight;
	public float MaxLightIntensity;
	public float IntensityLight;
	private float _priorIntensity;
	private Light _lightComponent;
	private float[] _rates = { 20f, 15f, 7f, 12f };
	void Start () {
		_lightComponent = TorchLight.GetComponent<Light>();
		_lightComponent.intensity = IntensityLight;
		_priorIntensity = IntensityLight;
    }

	public void SetComponentEmissions(ParticleSystem system, int index)
	{
        SetEmissionRate(system, _rates[index]);
    }
	private void SetEmissionRate(ParticleSystem system, float rate)
	{
		ParticleSystem.EmissionModule em = system.emission;
		em.rateOverTime = rate * IntensityLight; 
	}

	void Update () {
		if(_priorIntensity != IntensityLight)
		{
			_lightComponent.intensity = IntensityLight / 2f + Mathf.Lerp(IntensityLight - 0.1f, IntensityLight + 0.1f, Mathf.Cos(Time.time * 30));
			_priorIntensity = IntensityLight;
        }
	}
}
