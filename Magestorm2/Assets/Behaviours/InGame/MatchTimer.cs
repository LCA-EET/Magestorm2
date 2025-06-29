using System;
using TMPro;
using UnityEngine;

public class MatchTimer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float _secondsRemaining;
    private float _secondsElapsed = 0.0f;
    private float _elapsedSinceLastUpdate = 0.0f;
    private TMP_Text _timeText;

    private void Awake()
    {
        ComponentRegister.MatchTimer = this;
    }
    void Start()
    {
        _secondsRemaining = 3600.0f;
        _timeText = GetComponentInChildren<TMP_Text>();
        
    }

    // Update is called once per frame
    void Update()
    {
        _elapsedSinceLastUpdate += Time.deltaTime;
        if(_elapsedSinceLastUpdate >= 1.0f)
        {
            _secondsRemaining -= _elapsedSinceLastUpdate;
            _secondsElapsed += _elapsedSinceLastUpdate;
            _elapsedSinceLastUpdate = 0.0f;
            int minutesLeft = (int)Math.Floor(_secondsRemaining / 60);
            int secondsRemaining = (int)(Math.Floor(_secondsRemaining) - (minutesLeft * 60));
            //string toPrint
            string minutesLeftString, secondsRemainingString; 
            if(minutesLeft < 10)
            {
                minutesLeftString = "0" + minutesLeft;
            }
            else
            {
                minutesLeftString = minutesLeft.ToString();
            }
            if(secondsRemaining < 10)
            {
                secondsRemainingString = "0" + secondsRemaining;
            }
            else
            {
                secondsRemainingString = secondsRemaining.ToString();
            }
            _timeText.text = Language.BuildString(2, minutesLeftString, secondsRemainingString);
        }
        
    }
    
    public float SecondsRemaining{
        get
        {
            return _secondsRemaining;
        }
    }
    public float SecondsElapsed
    {
        get
        {
            return _secondsElapsed;
        }
    }
}
