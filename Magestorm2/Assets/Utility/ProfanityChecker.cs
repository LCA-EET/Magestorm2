using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
public static class ProfanityChecker
{
    private static HashSet<string> _prohibitedTerms;
    public static void Init()
    {
        _prohibitedTerms = new HashSet<string>();
        string path = Application.dataPath + "/prohibitedterms.txt";
        string[] terms = File.ReadAllLines(path);
        foreach (string term in terms)
        {
            _prohibitedTerms.Add(term);
        }
        LetterReplace("a", "4");
        LetterReplace("b", "8");
        LetterReplace("e", "3");
        LetterReplace("g", "6");
        LetterReplace("i", "1");
        LetterReplace("l", "1");
        LetterReplace("o", "0");
    }

    private static void LetterReplace(string letter, string number)
    {
        HashSet<string> additionalTerms = new HashSet<string>();
        foreach(string term in _prohibitedTerms)
        {
            additionalTerms.Add(term.Replace(letter, number));
        }
        foreach(string term in additionalTerms)
        {
            if (!_prohibitedTerms.Contains(term))
            {
                _prohibitedTerms.Add(term);
            }
        }
    }
    public static bool ContainsProhibitedLanguage(string text)
    {
        string lower = text.ToLower();
        foreach(string term in _prohibitedTerms)
        {
            if (lower.Contains(term))
            {
                return true;
            }
        }
        return false;
    }
}

