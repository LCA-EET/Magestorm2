using System.Collections.Generic;

public class HMLUpdater
{
    private float _elapsed;
    private float _period;
    private float _priorValue;
    private float _maxValue;
    private float _newValue;
    private float _currentValue;
    private bool _updateNeeded;
    private PlayerIndicator _barIndicator;
    private Dictionary<PlayerIndicator, HMLUpdater> _owner;
    public HMLUpdater(float period, float maxValue, PlayerIndicator indicator, Dictionary<PlayerIndicator, HMLUpdater> owner)
    {
        _period = period;
        _maxValue = maxValue;
        _barIndicator = indicator;
        _owner = owner;
        _owner.Add(_barIndicator, this);
    }

    public void UpdateIndication()
    {
        if (SharedFunctions.ProcessFloatLerp(ref _elapsed, _period, _priorValue, _newValue, ref _currentValue))
        {
            _updateNeeded = false;
        }
        ComponentRegister.PlayerStatusPanel.SetIndicator(_barIndicator, _currentValue / _maxValue);
    }
    public void UpdateValue(float newValue)
    {
        _priorValue = _currentValue;
        _newValue = newValue;
        _updateNeeded = newValue != _currentValue;
        if (_updateNeeded)
        {
            _elapsed = 0.0f;
        }
    }
    public float Value
    {
        get
        {
            return _newValue;
        }
    }
    public bool UpdateNeeded
    {
        get { return _updateNeeded; }
    }
}
