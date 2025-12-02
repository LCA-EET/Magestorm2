public class TimedEffect : AppliedEffect
{
    protected float _duration, _elapsed;
    public TimedEffect(EffectCode effectCode, float duration) : base(effectCode)
    {
        _duration = duration;
        _elapsed = 0;
    }
    public void TimeUpdate(float deltaTime)
    {
        _elapsed += deltaTime;
        if(_elapsed >= _duration)
        {
            ReverseEffect();
        }
    }
    protected void Update()
    {
    }
}
