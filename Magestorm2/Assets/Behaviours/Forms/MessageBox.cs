using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MessageBox : InstantiatableForm
{
    private TMP_Text _textBox;
    private Button _acknowledgeButton;
    private void Awake()
    {
        _textBox = GetComponentInChildren<TMP_Text>();
        _acknowledgeButton = GetComponentInChildren<Button>();
        _acknowledgeButton.onClick.AddListener(CloseForm);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void SetInstantiator(GameObject instantiator, object[] paramArray)
    {
        base.SetInstantiator(instantiator);
        _textBox.text = paramArray[0].ToString();
    }
}
