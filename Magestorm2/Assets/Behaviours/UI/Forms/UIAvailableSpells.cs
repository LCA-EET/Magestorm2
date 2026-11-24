using TMPro;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
namespace Magus
{

    public class UIAvailableSpells : ValidatableForm, ISpellProcessor
    {
        public TMP_Text FormHeader;
        public SpellSelectView SpellSelectView;

        private ISpellProcessor _owner;
        private byte _slotID;
        private List<SpellData> _spellList;
        private int _startIndex = 0;
        private void Start()
        {
            AssociateFormToButtons();
            Debug.Log("UIAvailableSpells form launched");
        }
        public void InitializeForm(byte characterLevel, byte slotID, ISpellProcessor owner, Dictionary<byte, byte> disciplineLevels)
        {
            _slotID = slotID;
            _owner = owner;
            FormHeader.text = Language.BuildString(277, slotID);
            Dictionary<byte, SpellData> availableSpells = Game.ActiveSpellManager.GetAvailableSpells(characterLevel, disciplineLevels);
            if(availableSpells.Count <= 10)
            {
                FormButtons[1].gameObject.SetActive(false);
                FormButtons[2].gameObject.SetActive(false);
            }
            _spellList = new List<SpellData>();
            foreach(SpellData spellData in availableSpells.Values)
            {
                _spellList.Add(spellData);
            }
            FillOptions();
        }
        private void FillOptions()
        {
            Dictionary<byte, short> options = new Dictionary<byte, short>();
            for (int i = _startIndex; i < _startIndex + 10; i++)
            {
                if (i < _spellList.Count)
                {
                    SpellData spellData = _spellList[i];
                    options.Add(spellData.SpellID, spellData.SpellNameReference);
                }
            }
            SpellSelectView.SetOwningForm(this);
            SpellSelectView.AssignKeys(options);
        }
        public override void ButtonPressed(ButtonType buttonType)
        {
            switch (buttonType)
            {
                case ButtonType.Cancel:
                    CloseForm();
                    break;
                case ButtonType.Misc0:
                    AdjustPage(-10);
                    break;
                case ButtonType.Misc1:
                    AdjustPage(10);
                    break;

            }
        }
        private void AdjustPage(int adjustment)
        {
            _startIndex += adjustment;
            if(_startIndex < 0)
            {
                _startIndex = (Mathf.FloorToInt(_spellList.Count / 10)) * 10;
            }
            else if (_startIndex > _spellList.Count) 
            {
                _startIndex = 0;
            }
            FillOptions();
        }

        public void SelectionMade(object[] args)
        {
            SpellData selected = (SpellData)args[0];
            _owner.SelectionMade(new object[] { _slotID, selected.SpellNameReference, selected.SpellID });
            CloseForm();
        }
    }

}