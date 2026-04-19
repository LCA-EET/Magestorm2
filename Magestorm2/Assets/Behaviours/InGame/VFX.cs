using UnityEngine;
public class VFX : MonoBehaviour
{
    public VFXCode VFXCode;
    public AudioSource AudioSource;
    public AudioClip AudioClip;
    public VFXDirection VFXDirection = VFXDirection.NA;
    private float _elapsed;
    public byte ExpireAfter = 3;
    public void Start()
    {
        if(AudioClip != null && AudioSource != null)
        {
            AudioSource.Play();
        }
        if(VFXDirection == VFXDirection.Down)
        {
            transform.localPosition =new Vector3(0,2,0);
        }
        else
        {
            transform.localPosition = Vector3.zero;
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
