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
        List<SpellData> spellData = DisciplineManager.GetSpellsOfDiscipline(new byte[] { (byte)disciplineCode });
        Dictionary<byte, int> options = new Dictionary<byte, int>();
        foreach(SpellData sd in spellData)
        {
            options.Add(sd.SpellID, sd.SpellNameReference);
        }
        AssignKeys(options);
        _selectedOption = Labels[0].OptionID;
        Labels[0].MarkSelected(true);
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
