using UnityEngine.UI;
using UnityEngine;
using TMPro;
public class ConsumableDisplay : MonoBehaviour
{
    public Image RedPotion, BluePotion, PurplePotion;
    public TMP_Text KeyString;
    private void Awake()
    {
        Game.ConsumableDisplay = this;
    }
    private void Start()
    {
        KeyString.color = Colors.GetTeamColor(MatchParams.MatchTeam);
        RefreshKey();
    }
    public void RefreshKey()
    {
        KeyString.text = InputControls.KeyToString(InputControl.UsePotion);
    }
    public void DisplayConsumable(byte id)
    {
        HideImage();
        switch (id)
        {
            case ControlCodes.Potion_Rejuvenation:
                PurplePotion.gameObject.SetActive(true); 
                break;
            case ControlCodes.Potion_Health:
                RedPotion.gameObject.SetActive(true);
                break;
            case ControlCodes.Potion_Mana:
                BluePotion.gameObject.SetActive(true);
                break;
        }
        if(id != ControlCodes.Potion_None)
        {
            KeyString.gameObject.SetActive(true);
        }    
    }

    public void HideImage()
    {
        RedPotion.gameObject.SetActive(false);
        BluePotion.gameObject.SetActive(false);
        PurplePotion.gameObject.SetActive(false);
        KeyString.gameObject.SetActive(false);
    }
}
