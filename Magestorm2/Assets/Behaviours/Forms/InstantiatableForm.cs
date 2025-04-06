using UnityEngine;

public class InstantiatableForm : MonoBehaviour
{
    protected GameObject _instantiator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public virtual void Reactivate(GameObject instantiated)
    {
        gameObject.SetActive(true);
        Destroy(instantiated);
    }
    public virtual void SetInstantiator(GameObject instantiator)
    {
        _instantiator = instantiator;
        Debug.Log("Instantiator set!");
    }
    public virtual void SetInstantiator(GameObject instantiator, object[] paramArray)
    {
        SetInstantiator(instantiator);  
    }
    public virtual void CloseForm()
    {
        ComponentRegister.UIPrefabManager.ActivateUIPrefab(_instantiator);
        ComponentRegister.UIPrefabManager.DestroyUIPrefab(gameObject);
    }
}
