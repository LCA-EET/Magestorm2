using UnityEngine;
using UnityEngine.TextCore.Text;
public class UISpellSlots : ValidatableForm, ISpellProcessor
{
    public SlotSelectView SlotSelectView;

    void Awake()
    {
        AssociateFormToButtons();
        PlayerCharacter pc = PlayerAccount.SelectedCharacter;
        SlotSelectView.Init(pc.SlottedSpells, pc.CharacterLevel, pc.DisciplineTable);
    }
    public void Start()
    {
    }
    public void SelectionMade(object[] args)
    {
        throw new System.NotImplementedException();
    }
    
}
