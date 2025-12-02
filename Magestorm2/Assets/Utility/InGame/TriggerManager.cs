using System.Collections.Generic;

public static class TriggerManager
{
    private static Dictionary<int, Trigger> _triggers;
    private static int _nextID = 0;
    public static void Init()
    {
        _triggers = new Dictionary<int, Trigger>();
    }

    public static int RegisterTrigger(Trigger toRegister)
    {
        _nextID++;
        if(_triggers == null)
        {
            Init();
        }
        _triggers.Add(_nextID, toRegister);
        return _nextID;
    }

    public static bool GetTrigger(int id, ref Trigger trigger)
    {
        if (_triggers.ContainsKey(id))
        {
            trigger = _triggers[id];
            return true;
        }
        return false;
    }
}
