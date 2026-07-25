using UnityEngine;

public class DamageDirectionPanel : MonoBehaviour
{
    public GameObject DDIPrefab;

    public void Awake()
    {
        ComponentRegister.DDIPanel = this;
    }
    public void InstantiateDDI(float angle)
    {
        //Debug.Log("Angle: " + angle);
        DamageDirectionIndicator ddi = Instantiate(DDIPrefab).GetComponent<DamageDirectionIndicator>();
        ddi.transform.parent = transform;
        ddi.transform.localPosition = Vector3.zero;
        ddi.transform.Rotate(new Vector3(0, 0, angle));
    }
}
