using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Magus
{

    public class UIMatchCreationForm : ValidatableForm
    {
        public MatchOptionSelection MatchOptionSelection;
        public LevelEntry[] LevelEntries;
        public SelectionGroup LevelSelection;
        public BitwiseToggleGroup DurationToggles, MatchTypeToggles;
        private bool _pmPacketSent;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            AssociateFormToButtons();
            PopulateLevelData();
            MatchParams.HashedMatchPassword = null;
        }
        protected override void Update()
        {
            base.Update();
            if(MatchParams.HashedMatchPassword != null && !_pmPacketSent)
            {
                _pmPacketSent = true;
                SendPrivateMatchPacket();
            }
        }
        public void SendPrivateMatchPacket()
        {
            int selectedIndex = LevelSelection.SelectedIndex;
            Game.SendPregameBytes(Pregame_Packets.CreatePrivateMatchPacket(LevelEntries[selectedIndex].LevelID, DurationToggles.GetSelectedIndex(), MatchTypeToggles.GetSelectedIndex(), MatchOptionSelection.GenerateValue()));
            CloseForm();
        }
        private void PopulateLevelData()
        {
            List<Level> levelList = LevelData.GetLevelList();
            int levelCount = levelList.Count;
            int index = 0;
            while (index < levelCount)
            {
                LevelEntries[index].AssociateLevel(levelList[index]);
                index++;
            }
            while (index < LevelEntries.Length)
            {
                LevelEntries[index].gameObject.SetActive(false);
                index++;
            }
        }
        public override void ButtonPressed(ButtonType buttonType)
        {
            if (buttonType == ButtonType.Submit)
            {
                int selectedIndex = LevelSelection.SelectedIndex;
                if (selectedIndex == -1)
                {
                    Game.MessageBox(Language.GetBaseString(60)); //
                }
                else
                {
                    LevelEntry selected = LevelEntries[selectedIndex];
                    if (MatchOptionSelection.IsOptionSelected(ControlCodes.MatchOptions_PrivateMatch))
                    {
                        ComponentRegister.UIPrefabManager.InstantiateMatchPasswordSetupForm();
                    }
                    else
                    {
                        Game.SendPregameBytes(Pregame_Packets.CreateMatchPacket(selected.LevelID, DurationToggles.GetSelectedIndex(), MatchTypeToggles.GetSelectedIndex(), MatchOptionSelection.GenerateValue()));
                        CloseForm();
                    }
                }
            }
            else
            {
                CloseForm();
            }
        }
    }

}