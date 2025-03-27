using UnityEngine;
using UnityEngine.UI;

public class BarIndicator : MonoBehaviour
{
    public Image FillImage;
    private Slider _slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetFillColor(Color color)
    {
        Debug.Log("COLOR SET");
        FillImage.color = color;
    }
    public void SetFill(float fill)
    {
        _slider.value = fill;
    }
}
