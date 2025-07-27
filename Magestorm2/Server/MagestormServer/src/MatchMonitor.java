import java.util.ArrayList;
import java.util.Collection;

public class MatchMonitor extends Thread{

    private final long _tick = 500;

    public MatchMonitor(){
        new Thread(this).start();
    }
    public void run(){
        while(Main.Running){
            try {
                CheckForExpiration();
                if(MatchManager.UpdatesNeeded){
                    MatchManager.NotifySubscribers();
                }
                Thread.sleep(_tick);
            } catch (InterruptedException e) {
                throw new RuntimeException(e);
            }
        }
    }
    private void CheckForExpiration()
    {
        long currentTime = System.currentTimeMillis();
        ArrayList<Match> activeMatches = MatchManager.GetMatches();
        for(Match match : activeMatches){
            if(currentTime >= match.GetExpiration()){
                match.MarkExpired();
            }
            else{
                match.Tick(_tick);
            }
        }
    }

}
