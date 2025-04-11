import java.io.File;
import java.io.FileNotFoundException;
import java.util.Dictionary;
import java.util.HashSet;
import java.util.Locale;
import java.util.Scanner;

public class ProfanityChecker {
    private static HashSet<String> _prohibitedTerms;

    public static void Init(String termsPath){
        _prohibitedTerms = new HashSet<>();
        File termsFile = new File(termsPath);
        try {
            Scanner termsScanner = new Scanner(termsFile);
            while(termsScanner.hasNextLine()){
                _prohibitedTerms.add(termsScanner.nextLine().toLowerCase(Locale.ROOT));
            }
        } catch (FileNotFoundException e) {
            throw new RuntimeException(e);
        }
        LetterReplace("a", "4");
        LetterReplace("b", "8");
        LetterReplace("e", "3");
        LetterReplace("g", "6");
        LetterReplace("i", "1");
        LetterReplace("l", "1");
        LetterReplace("o", "0");
        Main.LogMessage("Profanity list count: " + _prohibitedTerms.size());
    }
    private static void LetterReplace(String letter, String number){
        HashSet<String> additionalTerms = new HashSet<>();
        for (String prohibited : _prohibitedTerms){
            additionalTerms.add(prohibited.replace(letter, number));
        }
        for (String prohibited : additionalTerms){
            _prohibitedTerms.add(prohibited);
        }
    }
    public static boolean ContainsProhibitedLanguage(String text){
        for (String prohibited : _prohibitedTerms){
            if(text.contains(prohibited)){
                return true;
            }
        }
        return false;
    }
}
