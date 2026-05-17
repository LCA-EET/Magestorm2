using UnityEngine;

public class CharacterModel : MonoBehaviour
{
    private Vector3 _flyingRotation = new Vector3(30, 0, 0);

    public Transform HeadConnector;

    public void SetFlyingRotation()
    {
        Vector3 existing = gameObject.transform.eulerAngles;
        gameObject.transform.eulerAngles = new Vector3(30, existing.y, existing.z);
    }

    public void SetUprightRotation()
    {
        Vector3 existing = gameObject.transform.eulerAngles;
        gameObject.transform.eulerAngles = new Vector3(0, existing.y, existing.z);
    }
}
