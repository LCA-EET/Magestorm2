using TMPro;
using UnityEngine;

public class MessageDisplay : MonoBehaviour
{
    private TMP_Text _tmpText;
    private MessageData _messageData;
    private void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetMessage(MessageData messageData)
    {
        _messageData = messageData;
        _tmpText.text = _messageData.Message;
        _tmpText.color = _messageData.MessageColor;
    }
    public void ChangeOpacity(float opacity)
    {
        Color toAdjust = _tmpText.color;
        toAdjust.a = opacity;
        _tmpText.color = toAdjust;
    }
}
