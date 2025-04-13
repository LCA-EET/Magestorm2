using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectForm : ValidatableForm
{
    public CharacterCard[] CharacterCards;

    private void Awake()
    {
        List<PlayerCharacter> characterList = PlayerAccount.GetCharacterList();
        int cardIndex = 0;
        foreach (PlayerCharacter character in characterList)
        {
            CharacterCards[cardIndex].Populate(character);
            cardIndex++;
        }
        while(cardIndex < CharacterCards.Length)
        {
            CharacterCards[cardIndex].ActivatePanel(true);
            cardIndex++;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void CloseForm()
    {
        ComponentRegister.PregamePacketProcessor.SendBytes(Packets.LogOutPacket());
        base.CloseForm();
    }
}
