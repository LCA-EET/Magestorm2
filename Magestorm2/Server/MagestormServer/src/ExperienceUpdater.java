import java.sql.Connection;
import java.util.concurrent.LinkedBlockingQueue;

public class ExperienceUpdater extends RegisteredThread{
    private final LinkedBlockingQueue<MatchCharacter> _toProcess;
    private final Connection _persistentConnection;
    public ExperienceUpdater(){
        _toProcess = new LinkedBlockingQueue<>();
        _persistentConnection = Database.DBConnection();
        new RegisteredThread(this).start();
    }
    public void run(){
        while(!_terminated){
            try{
                UpdateExperience(_toProcess.take());

            }
            catch(InterruptedException ie){
                _terminated = true;
            }
        }
        Deregister();
    }
    private void UpdateExperience(MatchCharacter mc)
    {
        int experience = mc.GetEndingXP();
        if(experience > 0){
            int characterID = mc.GetCharacterID();

            PlayerCharacter pc = mc.PC();
            pc.UpdateExperience(experience);
            byte currentLevel = pc.GetCharacterLevel();
            byte newLevel = LevelData.DetermineLevel(currentLevel, experience);
            if(newLevel > currentLevel){
                pc.UpdateLevel(newLevel);
            }
            Database.UpdateExperience(characterID, experience, newLevel, _persistentConnection);
        }
    }
    public void AddToQueue(MatchCharacter mc){
        _toProcess.add(mc);
    }
}
