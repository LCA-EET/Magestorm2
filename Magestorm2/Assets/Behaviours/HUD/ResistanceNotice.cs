using TMPro;
using UnityEngine;
public class ResistanceNotice : MonoBehaviour
{
    public TMP_Text NoticeText;
    private Color _original;
    private bool _textShown;
    private float _elapsed;
    private void Awake()
    {
        ComponentRegister.ResistanceNotice = this;
    }
    private void Start()
    {
        SetColor(Colors.GetTeamColor(MatchParams.MatchTeam));
    }
    private void SetColor(Color color)
    {
        _original = color;
        NoticeText.color = _original;
    }
    public void SetText(string text)
    {
        _textShown = true;
        _elapsed = 0;
        NoticeText.text = text;
        NoticeText.color = _original;
    }
    public void Update()
    {
        if (_textShown)
        {
            _elapsed += Time.deltaTime;
            if(_elapsed > 5.0f)
            {
                ChangeOpacity((10.0f - _elapsed) / 5.0f);
                if(_elapsed > 10.0f)
                {
                    _textShown = false;
                }
            }
        }
        
    }
    private void ChangeOpacity(float opacity)
    {
        Color toAdjust = NoticeText.color;
        toAdjust.a = opacity;
        NoticeText.color = toAdjust;
    }
}
