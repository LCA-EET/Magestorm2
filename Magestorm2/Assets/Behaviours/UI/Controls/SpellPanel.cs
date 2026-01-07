using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
public class SpellPanel : MonoBehaviour
{
    private int _spellStringReference;
    public TMP_Text SpellText;

    public void Awake()
    {
        _spellStringReference = 295;
        ComponentRegister.SpellPanel = this;
    }

    public void UpdateSpellReference(int newReference)
    {
        SpellText.text = Language.BuildString(_spellStringReference, Language.GetBaseString(newReference));
    }
}
