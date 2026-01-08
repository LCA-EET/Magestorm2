using TMPro;
using UnityEngine;
public class SpellPanel : MonoBehaviour
{
    private int _spellStringReference, _secondaryReference;
    public TMP_Text SpellText, SecondaryText;

    public void Awake()
    {
        _spellStringReference = 296;
        _secondaryReference = 297;
        ComponentRegister.SpellPanel = this;
    }

    private void Start()
    {
        Color teamColor = Colors.GetTeamColor(MatchParams.MatchTeam);
        SpellText.color = teamColor;
        SecondaryText.color = teamColor;
        SpellText.text = Language.BuildString(_spellStringReference, Language.GetBaseString(276));
        SecondaryText.text = Language.BuildString(_secondaryReference, Language.GetBaseString(276));
    }
    public void UpdateSecondaryReference(int newReference)
    {
        SecondaryText.text = Language.BuildString(_secondaryReference, Language.GetBaseString(newReference));
    }
    public void UpdatePrimaryReference(int newReference)
    {
        SpellText.text = Language.BuildString(_spellStringReference, Language.GetBaseString(newReference));
    }
}
