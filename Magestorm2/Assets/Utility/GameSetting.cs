using System;
using UnityEngine;

public class GameSetting
{
    private string _keyString;
    private byte _value;
    private Action _onChange;
    public GameSetting(SettingKey key, byte defaultValue, Action onChange)
    {
        _keyString = key.ToString().ToLower() ;
        byte retrieved = (byte)PlayerPrefs.GetInt(_keyString);
        _onChange = onChange;
        if (retrieved == 0)
        {
            PlayerPrefs.SetInt(_keyString, defaultValue);
            Debug.Log("Setting default setting for " + _keyString + " = " + defaultValue);
            _value = defaultValue;
        }
        else
        {
            _value = retrieved;
            Debug.Log("Retrieved setting for " + _keyString + " = " + _value);
        }
    }
    public void WriteSetting()
    {
        PlayerPrefs.SetInt(_keyString , _value);
    }
    public void SetValue(byte newValue)
    {
        if (newValue != _value)
        {
            _value = newValue;
            ProcessChange();
        }
    }
    public byte Value
    {
        get { return _value; }
    }
    public void ProcessChange()
    {
        Debug.Log("ProcessChange()");
        if (_onChange != null)
        {
            _onChange();
        }
    }
}
