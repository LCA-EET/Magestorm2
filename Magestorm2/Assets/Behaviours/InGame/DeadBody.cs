using UnityEngine;

public class DeadBody : MonoBehaviour
{
    public RuntimeAnimatorController MaleDeath, FemaleDeath;
    private PeriodicAction _destroy;
    private bool _initialized;
    public void Initialize(GameObject model, Transform parent)
    {
        _destroy = new PeriodicAction(20.0f, DestroySelf, null);
        GameObject db = Instantiate(model);
        db.transform.parent = parent;
        
        _initialized = true;
    }
    private void Update()
    {
        if (_initialized)
        {
            _destroy.ProcessAction(Time.deltaTime);
        }
    }
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
