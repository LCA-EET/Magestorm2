using UnityEngine;
public class VFX : MonoBehaviour
{
    public byte VFXCode;
    public AudioSource AudioSource;
    public AudioClip AudioClip;
    private float _elapsed;
    public byte ExpireAfter = 3;
    public void Start()
    {
        if(AudioClip != null && AudioSource != null)
        {
            AudioSource.Play();
        }
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;
        if ((_elapsed >= ExpireAfter))
        {
            Destroy(gameObject);
        }
    }
}
