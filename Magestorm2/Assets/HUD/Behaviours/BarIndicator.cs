using UnityEngine;
using UnityEngine.UI;

public class BarIndicator : MonoBehaviour
{
    public Image FillImage;
    private Slider _slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _slider = GetComponent<Slider>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetFillColor(Color color)
    {
        FillImage.color = color;
    }
    public void SetFill(float fill)
    {
        _slider.value = fill;
    }
}
