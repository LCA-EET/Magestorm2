using UnityEngine;

public class Surface : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Collider Collider;
    public Footstep FootstepType;
    void Start()
    {
        //Debug.Log("SurfaceTest");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
    }
}
