using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MessageBox : MonoBehaviour
{
    private TMP_Text _textBox;
    private Button _acknowledgeButton;
    private GameObject _instantiator;
    private void Awake()
    {
        _textBox = GetComponentInChildren<TMP_Text>();
        _acknowledgeButton = GetComponentInChildren<Button>();
        _acknowledgeButton.onClick.AddListener(CloseMessageBox);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetMessageText(string text, GameObject owner)
    {
        _textBox.text = text;
        _instantiator = owner;
    }
    private void CloseMessageBox()
    {
        _instantiator.SetActive(true);
        Destroy(gameObject);
    }
}
