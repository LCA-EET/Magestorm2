using TMPro;
using UnityEngine;

public class LevelEntry : MonoBehaviour
{
    public TMP_Text LevelName, MaxPlayers;
    private Level _level;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AssociateLevel(Level toAssociate)
    {
        _level = toAssociate;
        LevelName.text = _level.LevelName;
        MaxPlayers.text = _level.MaxPlayers.ToString();
    }
    public byte LevelID
    {
        get { return _level.LevelID; }
    }
}
