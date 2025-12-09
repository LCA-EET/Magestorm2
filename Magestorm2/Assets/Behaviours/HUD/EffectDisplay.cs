using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EffectDisplay : MonoBehaviour
{
    public Image Image;
    private bool _isShown;
    private PeriodicAction _advanceSprite;
    private SpriteSet _spriteSet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _advanceSprite = new PeriodicAction(0.167f, AdvanceFrame, null);
    }
    void Start()
    {
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isShown)
        {
            _advanceSprite.ProcessAction(Time.deltaTime);
        }
    }
    public void Hide()
    {
        _isShown = false;
        Image.gameObject.SetActive(false); 
    }
    public void Show(bool show, SpriteSet spriteSet)
    {
        _spriteSet = spriteSet;
        Image.gameObject.SetActive(show);
        _isShown = show;
    }

    private void AdvanceFrame()
    {
        Image.sprite = _spriteSet.GetNextSprite();
    }
}
