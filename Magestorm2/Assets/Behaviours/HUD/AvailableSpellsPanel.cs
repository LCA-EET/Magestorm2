using System.Collections.Generic;
using UnityEngine;

public class AvailableSpellsPanel : MonoBehaviour
{
    public AvailableSpell[] AvailableSpells;
    private Dictionary<byte, AvailableSpell> _spellSelections;
    private PeriodicAction _msRefresh;
    private void Awake()
    {
        _spellSelections = new Dictionary<byte, AvailableSpell>();
        _msRefresh = new PeriodicAction(0.2f, RefreshMS, null);
        ComponentRegister.AvailableSpellsPanel = this;
    }

    private void Start()
    {
        RefeshPanel();
    }
    private void Update()
    {
        _msRefresh.ProcessAction(Time.deltaTime);
    }
    public void RefreshMS()
    {
        float currentMana = ComponentRegister.PC.CurrentMana;
        float currentStamina = ComponentRegister.PC.CurrentStamina; 
        foreach (AvailableSpell spell in AvailableSpells)
        {
            if (spell.IsEnabled)
            {
                spell.RefreshMS(currentMana, currentStamina);
            }
        }
    }
    public void RefeshPanel()
    {
        byte index = 0;
        byte[] slottedSpells = PlayerAccount.SelectedCharacter.SlottedSpells;
        for (byte b = 0; b < slottedSpells.Length; b++)
        {
            byte spellID = slottedSpells[b];
            if(spellID > 0)
            {
                AvailableSpell spell = AvailableSpells[index];
                //Debug.Log("Available Spell " + spell.name + " index: " + b);
                spell.SetAssociatedSpell(spellID);
                spell.MarkVisible(true);
                //Debug.Log("b: " + b + " slotIndex: " + slotIndex + ". " + spellID);
                spell.UpdateKeyText((byte)(b + 100));
                index++;
            }
        }
        for (byte b = index; b < AvailableSpells.Length; b++)
        {
            AvailableSpells[b].MarkVisible(false);
        }
    }
}
