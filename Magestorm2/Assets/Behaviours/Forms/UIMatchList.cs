using UnityEngine;

public class UIMatchList : MonoBehaviour
{
    public MatchEntry[] MatchEntries;
    private void Awake()
    {
        foreach (MatchEntry entry in MatchEntries)
        {
            entry.SetOwningList(this);
        }
    }
    public void EntrySelected(MatchEntry selectedEntry)
    {
        foreach (MatchEntry entry in MatchEntries)
        {
            entry.MarkSelected(false);
        }
        selectedEntry.MarkSelected(true);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
