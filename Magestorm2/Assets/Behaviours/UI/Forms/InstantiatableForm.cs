using UnityEngine;

public class InstantiatableForm : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
    }
    void Start()
    {
        ComponentRegister.UIPrefabManager.AddToStack(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public virtual void SetParams(object[] args)
    {

    }
    public virtual void CloseForm()
    {
        ComponentRegister.UIPrefabManager.PopFromStack();
    }
}
