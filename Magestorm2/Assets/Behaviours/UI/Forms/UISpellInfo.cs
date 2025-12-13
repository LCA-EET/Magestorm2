using System.Collections.Generic;
using UnityEngine;
public class UISpellInfo : ValidatableForm
{
    public ScrollSelectView Disciplines;
    public ScrollSelectView Spells;
    private Dictionary<byte, int> _disciplines;
    void Start()
    {
        _disciplines = new Dictionary<byte, int>();
        _disciplines.Add(0, 229); // fire law
        _disciplines.Add(1, 230); // ice law
        _disciplines.Add(2, 231); // earth law
        Disciplines.AssignKeys(_disciplines);
        AssociateFormToButtons();
    }

    public override void ButtonPressed(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Cancel:
                CloseForm();
                break;
        }
    }
}
