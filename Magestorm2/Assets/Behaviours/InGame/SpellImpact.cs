using UnityEngine;
public class SpellImpact : MonoBehaviour
{
    public float Lifetime;
    public AudioClip ImpactClip;
    public AudioSource AudioSource;
    private PeriodicAction _destroyObject;
    private void Awake()
    {
        _destroyObject = new PeriodicAction(Lifetime, DestroyObject, null);
    }
    private void Update()
    {
        _destroyObject.ProcessAction(Time.deltaTime);
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
