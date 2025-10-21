using TMPro;
using Unity.Collections;
using UnityEngine;

public class StatLine : ValidatableForm
{
    public PlayerStats Statistic;
    public TMP_Text StatText;
    public TMP_Text StatValue;
    private byte _stat;
    private StatPanel _owningPanel;

    private void Awake()
    {
        _stat = 15;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
        RefreshStatValue();
    }

    public void DisableButtons()
    {
        foreach(FormButton button in FormButtons)
        {
            button.gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void RefreshStatValue()
    {
        StatValue.text = _stat.ToString();
    }
    public void AssignOwner(StatPanel owner)
    {
        _owningPanel = owner;
    }
    private void AdjustStatistic(bool increase)
    {
        if (increase)
        {
            if(_stat >= 20)
            {
                Game.MessageBox(Language.GetBaseString(67)); //
            }
            else
            {
                _stat++;
            }
        }
        else
        {
            if(_stat <= 10)
            {
                Game.MessageBox(Language.GetBaseString(67)); //
            }
            else
            {
                _stat--;
            }
        }
        RefreshStatValue();
    }
    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType) 
        {
            case ButtonType.Increase:
                if(_owningPanel.StatTotal() >= 90)
                {
                    Game.MessageBoxReference(68);
                }
                else
                {
                    AdjustStatistic(true);
                }
                break;
            case ButtonType.Decrease:
                AdjustStatistic(false);
                break;
        }
        _owningPanel.RefreshTotal();
    }

    public byte Value
    {
        get { return _stat; }
        set {  _stat = value; }
    }
}
