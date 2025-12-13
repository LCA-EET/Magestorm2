using TMPro;
using UnityEngine;

public class PrefilledEntry : MonoBehaviour
{
    public TMP_Text FieldDescription, FieldValue;
    public void FillData(int valueReference)
    {
        FieldValue.text = Language.GetBaseString(valueReference);
    }
}
