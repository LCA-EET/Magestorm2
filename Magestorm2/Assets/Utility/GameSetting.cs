using System;
using UnityEngine;

public class GameSetting
{
    private SettingKey _key;
    private byte _value;
    private Action _onChange;
    public GameSetting(SettingKey key, byte defaultValue, Action onChange)
    {
        _key = key;
        byte retrieved = (byte)PlayerPrefs.GetInt(key.ToString());
        _onChange = onChange;
        if (PlayerPrefs.GetInt(key.ToString()) == 0)
        {
            PlayerPrefs.SetInt(key.ToString(), defaultValue);
            _value = defaultValue;
        }
        else
        {
            _value = retrieved;
        }
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
