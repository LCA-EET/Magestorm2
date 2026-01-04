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
        _disciplines.Add((byte)SpellDiscipline.FireLaw, 229); // fire law
        _disciplines.Add((byte)SpellDiscipline.IceLaw, 230); // ice law
        _disciplines.Add((byte)SpellDiscipline.EarthLaw, 231); // earth law
        _disciplines.Add((byte)SpellDiscipline.Brilliance, 232); // brialliance
        _disciplines.Add((byte)SpellDiscipline.Displacement, 233); // Displacement
        _disciplines.Add((byte)SpellDiscipline.Psionics, 234); // Psionics
        _disciplines.Add((byte)SpellDiscipline.Supplication, 235); // Supplication
        _disciplines.Add((byte)SpellDiscipline.Healing, 236); // Healing
        _disciplines.Add((byte)SpellDiscipline.Smiting, 237); // Wounding
        _disciplines.Add((byte)SpellDiscipline.ManaLaw, 238); // Mana Law
        _disciplines.Add((byte)SpellDiscipline.VoidLaw, 239); // Void Law
        _disciplines.Add((byte)SpellDiscipline.Sigils, 240); // Sigils
        Disciplines.AssignKeys(_disciplines);
        AssociateFormToButtons();
        Spells.SetOwningForm(this);
        Disciplines.RecordSelection((byte)SpellDiscipline.FireLaw);
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
