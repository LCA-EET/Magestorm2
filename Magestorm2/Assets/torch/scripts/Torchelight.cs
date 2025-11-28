using UnityEngine;

public class Torchelight : MonoBehaviour {
	
	public GameObject TorchLight;
	public GameObject MainFlame;
	public GameObject BaseFlame;
	public GameObject Etincelles;
	public GameObject Fumee;
	public float MaxLightIntensity;
	public float IntensityLight;
	private float _priorIntensity;
	private Light _lightComponent;

	void Start () {
		_lightComponent = TorchLight.GetComponent<Light>();
		_lightComponent.intensity = IntensityLight;
		_priorIntensity = IntensityLight;
		SetEmissionRate(MainFlame, 20f);
        SetEmissionRate(BaseFlame, 15f);
        SetEmissionRate(Etincelles, 7f);
        SetEmissionRate(Fumee, 12f);
    }

	private void SetEmissionRate(GameObject emitter, float rate)
	{
		ParticleSystem.EmissionModule em = MainFlame.GetComponent<ParticleSystem>().emission;
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
