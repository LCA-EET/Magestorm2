using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using UnityEditor;

public static class Language
{
    private static bool _init = false;

    private static int _selectedLanguage;
    private static StringBuilder _builder;
   
    private static Dictionary<int, Dictionary<int, string>> _languageStrings;
    private static Dictionary<int, string> _languageIndices;

    public static void Init()
    {
        if (!_init)
        {
            IngestLanguageFiles();
            _builder = new StringBuilder();
            SelectedLanguage = 0;
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
    public static string BuildString(int stringReference, object param)
    {
        return BuildString(GetBaseString(stringReference), param);
    }
    public static string BuildString(int stringReference, object param, object param2)
    {
        return BuildString(GetBaseString(stringReference), param, param2);
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
    public static bool Initialized
    {
        get
        {
            return _init;
        }
    }
    public static string GetBaseString(int stringIndex)
    {
        return _languageStrings[SelectedLanguage][stringIndex];
    }
    private static void IngestLanguageFiles()
    {
        string path = "Assets\\Resources\\lang";
        string[] files = Directory.GetFiles(path, "*.tra");
        _languageIndices = new Dictionary<int, string>();
        _languageStrings = new Dictionary<int, Dictionary<int, string>>();
        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i];
            string language = filePath.Substring(filePath.LastIndexOf("\\") + 1).Replace(".tra", "");
            _languageIndices.Add(i, language);
            Dictionary<int, string> indexedStrings = new Dictionary<int, string>();
            string[] lines = File.ReadAllLines(filePath);
            for (int j = 0; j < lines.Length; j++)
            {
                indexedStrings.Add(j, lines[j]);
            }
            _languageStrings.Add(i, indexedStrings);
        }
    }
}
