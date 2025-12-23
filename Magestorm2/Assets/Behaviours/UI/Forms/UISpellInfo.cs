using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class UISpellInfo : ValidatableForm, ISpellProcessor
{
    public TMP_Text SpellDescription;
    public DisciplineSelectView Disciplines;
    public SpellSelectView Spells;
    private Dictionary<byte, int> _disciplines;
    void Start()
    {
        _disciplines = new Dictionary<byte, int>();
        _disciplines.Add(0, 229); // fire law
        _disciplines.Add(1, 230); // ice law
        _disciplines.Add(2, 231); // earth law
        _disciplines.Add(3, 232); // brialliance
        _disciplines.Add(4, 233); // Displacement
        _disciplines.Add(5, 234); // Psionics
        _disciplines.Add(6, 235); // Supplication
        _disciplines.Add(7, 236); // Healing
        _disciplines.Add(8, 237); // Wounding
        _disciplines.Add(9, 238); // Mana Law
        _disciplines.Add(10, 239); // Void Law
        _disciplines.Add(11, 240); // Sigils
        Disciplines.AssignKeys(_disciplines);
        AssociateFormToButtons();
        Spells.SetOwningForm(this);
    }

    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Cancel:
                CloseForm();
                break;
        }
    }
    public void SelectionMade(object[] args)
    {
        SpellData spellData = (SpellData)args[0];
        SpellDescription.text = Language.GetBaseString(spellData.GetInt(SpellAttributes.SPELL_NAME_REFERENCE)) + "\n" +
            Language.GetBaseString(spellData.GetInt(SpellAttributes.DESCRIPTION));
    }
}
