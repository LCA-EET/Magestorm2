using TMPro;
using UnityEngine;
public class ClassLevelPanel : MonoBehaviour 
{
    public TMP_Text ClassText, LevelText, Experience;

    public void Init(PlayerCharacter character)
    {
        ClassText.text = character.CharacterClassString;
        LevelText.text = Language.BuildString(294, character.CharacterLevel.ToString());
        Experience.text = Language.BuildString(341, character.GetExperience());
        gameObject.SetActive(true);
    }
}
