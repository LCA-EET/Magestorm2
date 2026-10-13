using UnityEngine;

public class Gravity : MonoBehaviour
{
    private float _deltaY;
    public GameObject ToMove;
    private void Awake()
    {
        if(ToMove == null)
        {
            ToMove = transform.parent.gameObject;
        }
        _deltaY = ToMove.transform.position.y - transform.position.y;
    }

    private float GetDeltaY()
    {
        return ToMove.transform.position.y - transform.position.y;
    }
    private void Update()
    {
        float delta = GetDeltaY();
        if(delta != _deltaY)
        {
            Vector3 translation = new Vector3(0, _deltaY - delta, 0);
            ToMove.transform.Translate(translation);
            transform.Translate(translation * -1);
        }
    }
}
