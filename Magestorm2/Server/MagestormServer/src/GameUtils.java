import java.util.Random;

public class GameUtils {
    private static Random _random;

    public static void init(){
        _random = new Random();
    }

    public static short DiceRoll(int numSides, int numRolls){
        return DiceRoll(1, numSides, numRolls);
    }

    public static short DiceRoll(int minRoll, int maxRoll, int numRolls){
        int total = 0;
        byte roll = 0;
        while (roll < numRolls){
            total += _random.nextInt(minRoll, maxRoll+1);
            roll++;
        }
        return (short)total;
    }
}
