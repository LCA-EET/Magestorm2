import java.util.Random;

public class GameUtils {
    private static Random _random;

    public static void init(){
        _random = new Random();
    }

    public static short DiceRoll(byte numSides, byte numRolls){
        int total = 0;
        byte roll = 0;
        while (roll < numRolls){
            total += _random.nextInt(1, numSides);
        }
        return (short)total;
    }


}
