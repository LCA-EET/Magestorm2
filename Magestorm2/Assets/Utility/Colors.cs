using UnityEngine;

public static class Colors
{
    public static Color TeamSelected, TeamUnselected;
    public static Color EntrySelected, EntryUnselected, CardUnselected;
    private static bool _init = false;
    public static void Init()
    {
        if (!_init)
        {
            EntrySelected = new Color(0f, 1f, 0.125f, 0.4313f);
            EntryUnselected = new Color(0f, 0f, 0f, 0f);
            CardUnselected = new Color(1f, 1f, 1f, 1f);
            TeamSelected = new Color(1f, 1f, 1f, 1f);
            TeamUnselected = new Color(1f, 1f, 1f, 0.3f);
            _init = true;
        }
    }
    public static Color ApplySelectionHighlightColor(bool isSelected)
    {
        return isSelected ? EntrySelected : EntryUnselected;
    }
    public static Color ApplyCardSelectionColor(bool isSelected)
    {
        return isSelected ? EntrySelected : CardUnselected;
    }
}
