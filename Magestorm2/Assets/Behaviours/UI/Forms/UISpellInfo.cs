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
        _disciplines.Add(ControlCodes.SpellDiscipline_FireLaw, 229); // fire law
        _disciplines.Add(ControlCodes.SpellDiscipline_IceLaw, 230); // ice law
        _disciplines.Add(ControlCodes.SpellDiscipline_EarthLaw, 231); // earth law
        _disciplines.Add(ControlCodes.SpellDiscipline_Brilliance, 232); // brialliance
        _disciplines.Add(ControlCodes.SpellDiscipline_Displacement, 233); // Displacement
        _disciplines.Add(ControlCodes.SpellDiscipline_Psionics, 234); // Psionics
        _disciplines.Add(ControlCodes.SpellDiscipline_Supplication, 235); // Supplication
        _disciplines.Add(ControlCodes.SpellDiscipline_Healing, 236); // Healing
        _disciplines.Add(ControlCodes.SpellDiscipline_Smiting, 237); // Wounding
        _disciplines.Add(ControlCodes.SpellDiscipline_ManaLaw, 238); // Mana Law
        _disciplines.Add(ControlCodes.SpellDiscipline_VoidLaw, 239); // Void Law
        _disciplines.Add(ControlCodes.SpellDiscipline_Sigils, 240); // Sigils
        _disciplines.Add(ControlCodes.SpellDiscipline_SpiritLaw, 283); // Spirit Law
        _disciplines.Add(ControlCodes.SpellDiscipline_Barriers, 284); // Barriers
        _disciplines.Add(ControlCodes.SpellDiscipline_Shielding, 285); // Shielding
        Disciplines.AssignKeys(_disciplines);
        AssociateFormToButtons();
        Spells.SetOwningForm(this);
        Disciplines.RecordSelection(ControlCodes.SpellDiscipline_FireLaw);
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
        SpellDescription.text = Language.GetBaseString(spellData.SpellNameReference) + "\n" +
            Language.GetBaseString(spellData.DescriptionReference);
    }
}
