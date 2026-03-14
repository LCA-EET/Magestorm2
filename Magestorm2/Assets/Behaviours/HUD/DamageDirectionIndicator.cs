using UnityEngine.UI;
using UnityEngine;
public class DamageDirectionIndicator : MonoBehaviour {
    public Image Image;
    private Color _arrowColor;

    private float _timeRemaining;
    private float _maxTime;
    private void Awake()
    {
    }
    private void Start()
    {
        _arrowColor = Image.color;
        _timeRemaining = 2.0f;
        _maxTime = _timeRemaining;
    }
    private void Update()
    {
        _timeRemaining -= Time.deltaTime;
        if(_timeRemaining < 0)
        {
            Destroy(gameObject);
        }
        else
        {
            _arrowColor.a = _timeRemaining / _maxTime;
            Image.color = _arrowColor;
        }
    }
}
