using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    private TMP_Text _tmpText;
    private void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetMessage(string text, string sender, Color color)
    {
        float secondsElapsed = ComponentRegister.MatchTimer.SecondsElapsed;
        int minutesElapsed = Mathf.FloorToInt(secondsElapsed / 60.0f);
        int seconds = Mathf.FloorToInt(secondsElapsed % 60);
        string minuteString = ApplyPrefix(minutesElapsed);
        string secondString = ApplyPrefix(seconds);

        _tmpText.text = "[" + minuteString + ":" + secondString +"] " + sender + ": " + text;
        _tmpText.color = color;
    }
    private string ApplyPrefix(int elapsed)
    {
        string toReturn;
        if (elapsed == 0)
        {
            toReturn = "00";
        }
        else if (elapsed < 10)
        {
            toReturn = "0" + elapsed;
        }
        else
        {
            toReturn = elapsed.ToString();
        }
        return toReturn;
    }
}
