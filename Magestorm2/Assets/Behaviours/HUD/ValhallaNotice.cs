using UnityEngine;
public class ValhallaNotice : MonoBehaviour
{
    public GameObject TextObject;
    public void Awake()
    {
        ComponentRegister.ValhallaNotice = this;
    }

    public void Show(bool show)
    {
        TextObject.SetActive(show);
    }
}
