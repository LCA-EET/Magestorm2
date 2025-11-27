
using UnityEngine;

public class ColoredSprite : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public bool Randomize;
    public float ColorTransitionPeriod;
    private Vector3 _a, _b;
    private PeriodicAction _recolor;
    private float _transparency;
    public void Awake()
    {
        if (Randomize)
        {
            _transparency = SpriteRenderer.color.a;
            _b = RandomizeColor();
            _recolor = new PeriodicAction(ColorTransitionPeriod, Recolor, null);
        }
    }

    private void Update()
    {
        if (Randomize)
        {
            _recolor.ProcessAction(Time.deltaTime);
            Vector3 lerpColor = Vector3.Lerp(_a, _b, _recolor.PercentComplete);
            SpriteRenderer.color = new Color(lerpColor.x, lerpColor.y, lerpColor.z, _transparency);
        }
    }
    private void Recolor()
    {
        _a = new Vector3(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b);
        _b = RandomizeColor();
    }
    private Vector3 RandomizeColor()
    {
        return new Vector3(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
    }
}
