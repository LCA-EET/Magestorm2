using TMPro;
using UnityEngine;
public class SpellPanel : MonoBehaviour
{
    private int _spellStringReference, _secondaryReference;
    public TMP_Text SpellText, SecondaryText;
    private LanguageWrapper _primary, _secondary;
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
        _primary = Language.RegisterText(SpellText, _spellStringReference, 276);
        _secondary = Language.RegisterText(SecondaryText, _secondaryReference, 276);
    }
    public void UpdateSecondaryReference(int newReference)
    {
        _secondary.UpdateParameterIndex(newReference);
    }
    public void UpdatePrimaryReference(int newReference)
    {
        _primary.UpdateParameterIndex(newReference);
    }
}
