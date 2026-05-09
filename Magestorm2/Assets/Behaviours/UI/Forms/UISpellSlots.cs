public class UISpellSlots : ValidatableForm, ISpellProcessor
{
    public SlotSelectView SlotSelectView;

    void Awake()
    {
        AssociateFormToButtons();
        
    }
    public void Start()
    {
        PlayerCharacter pc = PlayerAccount.SelectedCharacter;
        SlotSelectView.Init(pc.SlottedSpells, pc.CharacterLevel, pc.DisciplineTable);
    }
    public void SelectionMade(object[] args)
    {
        //throw new System.NotImplementedException();
    }
    protected override void PassedValidation()
    {
        byte[] selections = SlotSelectView.SlotSelections;
        Game.SendInGameBytes(InGame_Packets.UpdateSlots(selections));
        PlayerAccount.SelectedCharacter.UpdateSlottedSpells(selections, 0);
        CloseForm();
    }
}
