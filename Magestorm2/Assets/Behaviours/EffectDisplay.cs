using UnityEngine;
using UnityEngine.UI;

public class EffectDisplay : MonoBehaviour
{
    private Image _image;
    private bool _isShown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _image = GetComponentInChildren<Image>();
    }
    void Start()
    {
        Show(false, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(bool show, byte effectID)
    {
        _image.gameObject.SetActive(show);
        _isShown = show;
    }

}
