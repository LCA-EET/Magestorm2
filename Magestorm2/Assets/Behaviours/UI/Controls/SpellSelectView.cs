
using System.Collections.Generic;

public class SpellSelectView : ScrollSelectView
{
    private ISpellProcessor _owner;
    public override void Start()
    {
        base.Start();
    }

    public void SetOwningForm(ISpellProcessor owner)
    {
        _owner = owner;
    }

    public void PopulateOptions(byte disciplineCode)
    {
        List<SpellData> spellData = SpellManager.GetSpellsOfDiscipline(new SpellDiscipline[] { (SpellDiscipline)disciplineCode });
        Dictionary<byte, int> options = new Dictionary<byte, int>();
        foreach(SpellData sd in spellData)
        {
            options.Add(sd.GetByte(SpellAttributes.ID), sd.GetInt(SpellAttributes.SPELL_NAME_REFERENCE));
        }
        AssignKeys(options);
        _selectedOption = Labels[0].OptionID;
        ProcessSelection();
    }

    protected override void ProcessSelection()
    {
        SpellData reference = null;
        if(SpellManager.GetSpell(_selectedOption, ref reference))
        {
            _owner.SelectionMade(new object[] { reference });
        }
    }
}
