using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterSelectForm : ValidatableForm
{
    public UICharacterCard[] CharacterCards;
    public RenderTexture[] RenderTextures;
    private void Awake()
    {
        for (int i = 0; i < CharacterCards.Length; i++)
        {
            CharacterCards[i].SetOwningForm(this, RenderTextures[i]);
        }
        RefreshCards();
        ReselectExisting();
    }
    private void ReselectExisting()
    {
        if (PlayerAccount.SelectedCharacter != null)
        {
            int charID = PlayerAccount.SelectedCharacter.CharacterID;
            foreach (UICharacterCard card in CharacterCards)
            {
                if (card.Populated)
                {
                    if(charID == card.CharacterID())
                    {
                        card.MarkSelected(true);
                        return;
                    }
                }
            }
            PlayerAccount.SelectedCharacter = null;
        }
    }
    public void RefreshCards()
    {
        
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
        InputControls.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerAccount.UpdatesMade)
        {
            RefreshCards();
            DeselectCards();
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
        Game.SendPregameBytes(Pregame_Packets.SubscribeToMatchesPacket());
    }
    protected override bool ValidateForm()
    {
        return PlayerAccount.SelectedCharacter != null;
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
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
            case ButtonType.Edit:
                ComponentRegister.UIPrefabManager.InstantiateKeyMapper();
                break;
        }
    }
    public override void CloseForm()
    {
        ComponentRegister.PregamePacketProcessor.SendBytes(Pregame_Packets.LogOutPacket());
        base.CloseForm();
    }
}
