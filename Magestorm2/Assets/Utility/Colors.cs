using UnityEngine;

public static class Colors
{
    public static Color EntrySelected, EntryUnselected, CardUnselected;

    public static void Init()
    {
        EntrySelected = new Color(0f, 1f, 0.125f, 0.4313f);
        EntryUnselected = new Color(0f, 0f, 0f, 0f);
        CardUnselected = new Color(1f, 1f, 1f, 1f);
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
