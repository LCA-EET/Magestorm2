using UnityEngine;

public class DeadBody : MonoBehaviour
{
    public RuntimeAnimatorController MaleDeath, FemaleDeath;
    private PeriodicAction _destroy;
    private bool _initialized, _shrink;
    private float _shrinkElapsed;
    public void Initialize(GameObject model, Transform parent)
    {
        _destroy = new PeriodicAction(10.0f, DestroySelf, null);
        GameObject db = Instantiate(model);
        db.transform.parent = parent;
        
        _initialized = true;
    }
    private void Update()
    {
        if (_initialized && !_shrink)
        {
            _destroy.ProcessAction(Time.deltaTime);
        }
        if (_shrink)
        {
            if(_shrinkElapsed >= 1.0f)
            {
                Destroy(gameObject);
            }
            else
            {
                _shrinkElapsed += Time.deltaTime;
                float shrinkFactor = 1.0f - _shrinkElapsed;
                transform.localScale = new Vector3(shrinkFactor, shrinkFactor, shrinkFactor);
            }
        }
        //see if the player tapped or was rezzed;
    }
    public void DestroySelf()
    {
        _shrink = true; 
    }
}
