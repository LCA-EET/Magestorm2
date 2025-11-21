using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MessageDisplay : MonoBehaviour
{
    private MessageData _messageData;
    private Image _image;
    private Color _imageColor;
    public TMP_Text TextObject;
    public GameObject Background;

    private void Start()
    {
        _image = Background.GetComponent<Image>();
        _imageColor = new Color(Colors.TextBackground.r, Colors.TextBackground.g, Colors.TextBackground.b, Colors.TextBackground.a);
    }
    public void SetMessage(MessageData messageData)
    {
        _messageData = messageData;
        TextObject.text = _messageData.Message;
        TextObject.color = _messageData.MessageColor;
        _imageColor.a = 0.5f;
        _image.color = _imageColor;
    }
    public void ChangeOpacity(float opacity)
    {
        Color toAdjust = TextObject.color;
        toAdjust.a = opacity;
        TextObject.color = toAdjust;

        _imageColor.a = opacity * 0.5f;
        _image.color = _imageColor;
    }
}
