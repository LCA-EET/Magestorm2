using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using TMPro;

public static class Language
{
    private static bool _init = false;

    private static int _selectedLanguage;
    private static StringBuilder _builder;
   
    private static Dictionary<int, Dictionary<int, string>> _languageStrings;
    private static List<LanguageUpdater> _languageUpdaters;
    private static List<LanguageWrapper> _languageWrappers;
    public static void Init()
    {
        if (!_init)
        {
            _languageUpdaters = new List<LanguageUpdater>();
            _languageWrappers = new List<LanguageWrapper>();
            IngestLanguageFiles();
            _builder = new StringBuilder();

            SelectedLanguage = GameSettings.GetSettingValue(SettingKey.Language);
            Debug.Log("SELECTED LANGUAGE: " + SelectedLanguage);
            _init = true;
        }
    }
    public static int SelectedLanguage
    {
        get
        {
            return _selectedLanguage;
        }
        set
        {
            _selectedLanguage = value;
        }
    }
    public static void SetLanguage(byte language)
    {
        _selectedLanguage = language;
        foreach (LanguageUpdater updater in _languageUpdaters)
        {
            try
            {
                updater.UpdateLanguage();
            }
            catch(Exception ex)
            {

            }
        }
        foreach(LanguageWrapper wrapper in _languageWrappers)
        {
            try
            {
                wrapper.UpdateLanguage();
            }
            catch(Exception ex)
            {

            }
        }
    }
    public static string BuildIndexedString(int stringReference, int paramIndex)
    {
        _builder.AppendFormat(GetBaseString(stringReference), GetBaseString(paramIndex));
        string toReturn = _builder.ToString();
        _builder.Clear();
        return toReturn;
    }
    public static string BuildIndexedString(int stringReference, int paramIndex, int paramIndex2)
    {
        _builder.AppendFormat(GetBaseString(stringReference), GetBaseString(paramIndex), GetBaseString(paramIndex2));
        string toReturn = _builder.ToString();
        _builder.Clear();
        return toReturn;
    }
    public static string BuildIndexedString(int stringReference, int paramIndex, int paramIndex2, int paramIndex3)
    {
        _builder.AppendFormat(GetBaseString(stringReference), GetBaseString(paramIndex), GetBaseString(paramIndex2), GetBaseString(paramIndex3));
        string toReturn = _builder.ToString();
        _builder.Clear();
        return toReturn;
    }
    public static string BuildString(int stringReference, object param)
    {
        return BuildString(GetBaseString(stringReference), param);
    }
    public static string BuildString(int stringReference, object param, object param2)
    {
        return BuildString(GetBaseString(stringReference), param, param2);
    }
    public static string BuildString(int stringReference, object param, object param2, object param3)
    {
        return BuildString(GetBaseString(stringReference), param, param2, param3);
    }
    public static string BuildString(string baseString, object param)
    {
        _builder.AppendFormat(baseString, param);
        string toReturn = _builder.ToString();
        _builder.Clear();
        return toReturn;
    }
    public static string BuildString(string baseString, object param1, object param2)
    {
        _builder.AppendFormat(baseString, param1, param2);
        string toReturn = _builder.ToString();
        _builder.Clear();
        return toReturn;
    }
    public static string BuildString(string baseString, object param1, object param2, object param3)
    {
        _builder.AppendFormat(baseString, param1, param2, param3);
        string toReturn = _builder.ToString();
        _builder.Clear();
        return toReturn;
    }
    public static bool Initialized
    {
        get
        {
            return _init;
        }
    }
    public static string GetBaseString(int stringIndex)
    {
        if (_languageStrings.ContainsKey(SelectedLanguage))
        {
            Dictionary<int, string> lang = _languageStrings[SelectedLanguage];
            if (lang.ContainsKey(stringIndex))
            {
                return lang[stringIndex];
            }
            else
            {
                if (_languageStrings[ControlCodes.Language_English].ContainsKey(stringIndex))
                {
                    return _languageStrings[ControlCodes.Language_English][stringIndex];
                }
                else
                {
                    Debug.Log("No language string for index " + stringIndex);
                    return "[undef]";
                }
            }
        }
        else
        {
            Debug.Log("Invalid language selection: " + SelectedLanguage);
            return "[undef]";
        }
        
    }
    private static void IngestLanguageFiles()
    {
        string path = Application.streamingAssetsPath +"\\lang";
        string[] files = Directory.GetFiles(path, "*.tra");
        _languageStrings = new Dictionary<int, Dictionary<int, string>>();
        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i];
            string language = filePath.Substring(filePath.LastIndexOf("\\") + 1).Replace(".tra", "");
            Dictionary<int, string> indexedStrings = new Dictionary<int, string>();
            string[] lines = File.ReadAllLines(filePath);
            for (int j = 1; j <= lines.Length; j++)
            {
                indexedStrings.Add(j, lines[j-1]);
            }
            _languageStrings.Add(i+1, indexedStrings);
            Debug.Log("Loaded language " + language + " at index " + (i + 1));
        }
    }
    public static void RegisterLanguageUpdater(LanguageUpdater languageUpdater)
    {
        _languageUpdaters.Add(languageUpdater);
    }
    public static LanguageWrapper RegisterText(TMP_Text text, int baseIndex, int paramIndex1)
    {
        LanguageWrapper toAdd = new LanguageWrapper(text, baseIndex, paramIndex1);
        _languageWrappers.Add(toAdd);
        toAdd.UpdateLanguage();
        return toAdd;
    }
    public static void RegisterText(TMP_Text text, int baseIndex)
    {
        LanguageWrapper toAdd = new LanguageWrapper(text, baseIndex);
        _languageWrappers.Add(toAdd);
        toAdd.UpdateLanguage();
    }
    public static LanguageWrapper RegisterText(TMP_Text text, int baseIndex, string param1)
    {
        LanguageWrapper toAdd = new LanguageWrapper(text, baseIndex, param1);
        _languageWrappers.Add(toAdd);
        toAdd.UpdateLanguage();
        return toAdd;
    }

    public static void RegisterText(TMP_Text text, int baseIndex, string param1, string param2)
    {
        LanguageWrapper toAdd = new LanguageWrapper(text, baseIndex, param1, param2);
        _languageWrappers.Add(toAdd);
        toAdd.UpdateLanguage();
    }

    public static void RegisterText(TMP_Text text, int baseIndex, string param1, string param2, string param3)
    {
        LanguageWrapper toAdd = new LanguageWrapper(text, baseIndex, param1, param2, param3);
        _languageWrappers.Add(toAdd);
        toAdd.UpdateLanguage();
    }
}
