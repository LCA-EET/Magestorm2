using UnityEngine;

public class DamagePanel : MonoBehaviour
{
    private bool _show;
    private float _elapsed;
    
    public void Show(bool value)
    {
        _show = value;
        gameObject.SetActive(_show);
    }
    private void Update()
    {
        if (_show)
        {
            _elapsed += Time.deltaTime;
            if(_elapsed > 0.05f)
            {
                _elapsed = 0;
                Show(false);
            }
        }
    }
}
