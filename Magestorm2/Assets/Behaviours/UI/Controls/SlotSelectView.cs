using UnityEngine;
public class SlotSelectView : ScrollSelectView, ISpellProcessor
{
    private byte _characterLevel;
    private SkillPanel _skillPanel;
    public void Init(byte[] slottedSpells, byte characterLevel, SkillPanel skillPanel)
    {
        _skillPanel = skillPanel;
        _characterLevel = characterLevel;
        for (byte i = 0; i < slottedSpells.Length; i++)
        {
            byte spellID = slottedSpells[i];
            int referenceID = 276;
            SpellData slottedSpell = null;
            if (SpellManager.GetSpell(spellID, ref slottedSpell))
            {
                referenceID = slottedSpell.GetInt(SpellAttributes.SPELL_NAME_REFERENCE);
            }
            Labels[i].Register(referenceID, i, this);
        }
    }

    public void SelectionMade(object[] args)
    {
        throw new System.NotImplementedException();
    }

    protected override void ProcessSelection()
    {
        ComponentRegister.UIPrefabManager.InstantiateAvailableSpellList(_characterLevel, _selectedOption, this, _skillPanel.GetDisciplineTable());
    }
}
