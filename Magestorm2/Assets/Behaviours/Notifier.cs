using TMPro;
using UnityEngine;

public class Notifier : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private TMP_Text _notifierText;
    private float _secondsRemaining;
    private Color _color;
    void Awake()
    {
        _notifierText = GetComponentInChildren<TMP_Text>();
    }
    void Start()
    {
        _color = Color.white;
        ComponentRegister.Notifier = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(_secondsRemaining > 0.0f)
        {
            _secondsRemaining -= Time.deltaTime;
            if (_secondsRemaining < 5.0f)
            {
                _notifierText.color = new Color(_color.r, _color.g, _color.b, _secondsRemaining / 5.0f);
            }
        }
    }

    public void DisplayNotification(string text)
    {
        _notifierText.text = text;
        _color = Color.white;
        _notifierText.color = _color;
        _secondsRemaining = 10.0f;
    }
}
