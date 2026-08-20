using UnityEngine;
public class VFX : MonoBehaviour
{
    public byte VFXCode;
    public AudioSource AudioSource;
    public AudioClip AudioClip;
    public VFXDirection VFXDirection = VFXDirection.NA;
    private float _elapsed;
    public byte ExpireAfter = 3;
    public void Start()
    {
        if(AudioClip != null && AudioSource != null)
        {
            AudioSource.clip = AudioClip;
            AudioSource.Play();
        }
        if(VFXDirection == VFXDirection.Down)
        {
            Vector3 adjusted = transform.localPosition;
            adjusted.y += 2.0f;
            transform.localPosition = adjusted;
        }
    }

    private void Update()
    {
        if(ExpireAfter > 0)
        {
            _elapsed += Time.deltaTime;
            if ((_elapsed >= ExpireAfter))
            {
                Destroy(gameObject);
            }
        }
    }
}
