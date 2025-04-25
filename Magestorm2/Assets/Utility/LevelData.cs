using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class LevelData
{
    private static Dictionary<byte, Level> _levelTable;
    public static void Init()
    {
        _levelTable = new Dictionary<byte, Level>();
    }
    public static int LevelCount
    {
        get { return _levelTable.Count; }
    }
    public static void AddLevel(byte id, byte maxPlayers, string levelName)
    {
        Level toAdd = new Level(id, levelName, maxPlayers);
        if (!_levelTable.ContainsKey(id))
        {
            _levelTable.Add(id, toAdd);
        }
    }
    public static List<Level> GetLevelList()
    {
        return _levelTable.Values.ToList();
    }
}
