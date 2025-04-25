using UnityEngine;

public class SelectionGroup : MonoBehaviour
{
    public SelectEntryButton[] SelectionEntries;
    private int _selectedIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _selectedIndex = -1;
        for (int i = 0; i < SelectionEntries.Length; i++)
        {
            SelectEntryButton button = SelectionEntries[i];
            button.SetOwner(this, i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DeselectAll()
    {
        foreach (SelectEntryButton selectEntryButton in SelectionEntries)
        {
            selectEntryButton.Selected = false;
        }
        _selectedIndex = -1;
    }
    public void DeselectOthers(SelectEntryButton selected)
    {
        DeselectAll();
        selected.Selected = true;
        _selectedIndex = selected.SelectionIndex;
    }
    public int SelectedIndex
    {
        get { return _selectedIndex; }
    }
    
}
