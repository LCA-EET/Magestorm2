using TMPro;
using System.Collections.Generic;
public class LanguageWrapper 
{
    private TMP_Text _text;
    private int _baseIndex;
    private List<int> _paramIndices;
    private List<string> _paramStrings;

    public LanguageWrapper(TMP_Text text, int baseIndex)
    {
        _text = text;
        _baseIndex = baseIndex;
        _paramIndices = new List<int>();
        _paramStrings = new List<string>();
    }
    
    public LanguageWrapper(TMP_Text text, int baseIndex, string paramIndex1) : this(text, baseIndex) 
    {
        _paramStrings.Add( paramIndex1);
    }

    public LanguageWrapper(TMP_Text text, int baseIndex, string paramIndex1, string paramIndex2) : this(text, baseIndex, paramIndex1)
    {
        _paramStrings.Add(paramIndex2);
    }

    public LanguageWrapper(TMP_Text text, int baseIndex, string paramIndex1, string paramIndex2, string paramIndex3) : this(text, baseIndex, paramIndex1, paramIndex2)
    {
        _paramStrings.Add(paramIndex3);
    }

    public LanguageWrapper(TMP_Text text, int baseIndex, int paramIndex1) : this(text, baseIndex)
    {
        _paramIndices.Add(paramIndex1);
    }

    public LanguageWrapper(TMP_Text text, int baseIndex, int paramIndex1, int paramIndex2) : this(text, baseIndex, paramIndex1)
    {
        _paramIndices.Add(paramIndex2);
    }

    public LanguageWrapper(TMP_Text text, int baseIndex, int paramIndex1, int paramIndex2, int paramIndex3) : this(text, baseIndex, paramIndex1, paramIndex2)
    {
        _paramIndices.Add(paramIndex3);
    }

    public void UpdateLanguage()
    {
        string text;
        switch (_paramStrings.Count)
        {
            case 1:
                text = Language.BuildString(_baseIndex, _paramStrings[0]);
                break;
            case 2:
                text = Language.BuildString(_baseIndex, _paramStrings[0], _paramStrings[1]);
                break;
            case 3:
                text = Language.BuildString(_baseIndex, _paramStrings[0], _paramStrings[1], _paramStrings[2]);
                break;
            default:
                text = Language.GetBaseString(_baseIndex);
                break;
        }
        switch (_paramIndices.Count)
        {
            case 1:
                text = Language.BuildIndexedString(_baseIndex, _paramIndices[0]);
                break;
            case 2:
                text = Language.BuildIndexedString(_baseIndex, _paramIndices[0], _paramIndices[1]);
                break;
            case 3:
                text = Language.BuildIndexedString(_baseIndex, _paramIndices[0], _paramIndices[1], _paramIndices[2]);
                break;
        }
        _text.text = text;
    }
    public void UpdateParameterIndex(int paramIdx)
    {
        _paramIndices[0] = paramIdx;
        _text.text = Language.BuildIndexedString(_baseIndex, _paramIndices[0]);
    }

    
}
