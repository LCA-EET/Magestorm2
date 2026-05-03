public class LevelData {
    private static final int[] _experience =
            {
                    0,      // 1
                    1000,   // 2
                    3000,   // 3
                    6000,   // 4
                    10000,  // 5
                    15000,  // 6
                    21000,  // 7
                    28000,  // 8
                    44000,  // 9
                    61000,  // 10
                    79000,  // 11
                    98000,  // 12
                    118000, // 13
                    139000, // 14
                    161000, // 15
                    184000, // 16
                    224000, // 17
                    265000, // 18
                    307000, // 19
                    350000, // 20
                    394000, // 21
                    439000, // 22
                    485000, // 23
                    532000, // 24
                    604000, // 25
                    677000, // 26
                    751000, // 27
                    826000, // 28
                    902000, // 29
                    979000  // 30
            };
    public static byte DetermineLevel(byte currentLevel, int experience){
        byte level = currentLevel;
        for(byte b = currentLevel; b < _experience.length; b++){
            if(_experience[b] <= experience){
                level = b;
            }
            else{
                return level;
            }
        }
        level++;
        return level;
    }
}
