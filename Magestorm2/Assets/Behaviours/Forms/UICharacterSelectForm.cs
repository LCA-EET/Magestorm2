using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterSelectForm : ValidatableForm
{
    public UICharacterCard[] CharacterCards;

    private void Awake()
    {
        foreach (UICharacterCard card in CharacterCards)
        {
            card.SetOwningForm(this);
        }
        RefreshCards();
    }
    private void RefreshCards()
    {
        DeselectCards();
        List<PlayerCharacter> characterList = PlayerAccount.GetCharacterList();
        int cardIndex = 0;
        foreach (PlayerCharacter character in characterList)
        {
            CharacterCards[cardIndex].Populate(character);
            cardIndex++;
        }
        while (cardIndex < CharacterCards.Length)
        {
            CharacterCards[cardIndex].ActivatePanel(true);
            cardIndex++;
        }
        PlayerAccount.UpdatesMade = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerAccount.UpdatesMade)
        {
            RefreshCards();
        }
    }
    private void DeselectCards()
    {
        foreach (UICharacterCard card in CharacterCards)
        {
            card.MarkSelected(false);
        }
    }
    public void CardSelected(UICharacterCard selected)
    {
        DeselectCards();
        selected.MarkSelected(true);   
    }
    protected override void PassedValidation()
    {
        ComponentRegister.UIPrefabManager.InstantiateMatchList();
    }
    protected override bool ValidateForm()
    {
        return PlayerAccount.SelectedCharacter != null;
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        UIAudio.PlayButtonPress();
        switch (buttonType)
        {
            case ButtonType.Cancel:
                CloseForm();
                break;
            case ButtonType.Submit:
                if (ValidateForm())
                {
                    PassedValidation();
                }
                else
                {
                    Game.MessageBox(Language.GetBaseString(35));
                }
                break;
        }
    }
    public override void CloseForm()
    {
        ComponentRegister.PregamePacketProcessor.SendBytes(Packets.LogOutPacket());
        base.CloseForm();
    }
}
