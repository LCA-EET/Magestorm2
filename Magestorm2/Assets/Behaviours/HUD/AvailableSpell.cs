using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AvailableSpell : MonoBehaviour
{
    public TMP_Text KeyText;
    public Image Icon;
    public Slider Slider;

    private SpellData _associatedSpell;
    private byte _manaRequired;
    private float _staminaRequired;
    private bool _enabled;
    public void UpdateKeyText(byte keyCode)
    {
        KeyText.text = InputControls.GetKeyCode(keyCode).ToString();
    }
    public bool IsEnabled
    {
        get { return _enabled; }
    }
    public void MarkVisible(bool visible)
    {
        gameObject.SetActive(visible);
       _enabled = visible;
    }
    
    public void SetAssociatedSpell(byte spellID)
    {
        if (SpellManager.GetSpell(spellID, ref _associatedSpell))
        {
            Icon.sprite = SpellIcons.GetIcon(spellID);
            //Debug.Log("ICON: " + Icon.sprite.ToString() + " " + Icon.name);
            _manaRequired = _associatedSpell.SpellCost;
            _staminaRequired = _associatedSpell.GetStaminaCost(PlayerAccount.SelectedCharacter.CharacterLevel);
        }
    }
    public void RefreshMS(float currentMana, float currentStamina)
    {
        Icon.color = (currentMana >= _manaRequired && currentStamina > _staminaRequired) ? Colors.Neutral : Colors.UnavailableSpell;
    }
}
