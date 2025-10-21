using System;
using TMPro;
using UnityEngine;

public class MatchTimer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float _elapsedSinceLastUpdate = 0.0f;
    private float _secondsElapsed = 0.0f;
    private TMP_Text _timeText;
    private string _timeString;

    private void Awake()
    {
        ComponentRegister.MatchTimer = this;
        _timeString = Language.GetBaseString(3); //
    }
    void Start()
    {
        _timeText = GetComponentInChildren<TMP_Text>();
        RefreshTime();
    }

    // Update is called once per frame
    void Update()
    {
        _elapsedSinceLastUpdate += Time.deltaTime;
        _secondsElapsed += Time.deltaTime; 
        if(_elapsedSinceLastUpdate >= 1.0f)
        {
            _elapsedSinceLastUpdate = 0.0f;
            RefreshTime();
        }
    }
    private void RefreshTime()
    {
        _timeText.text = _timeString + TimeUtil.MinutesAndSecondsRemaining(MatchParams.ExpirationTime);
    }
    public float SecondsElapsed
    {
        get
        {
            return _secondsElapsed;
        }
    }
}
