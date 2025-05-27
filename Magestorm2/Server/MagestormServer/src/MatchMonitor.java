import java.util.ArrayList;
import java.util.Collection;

public class MatchMonitor extends Thread{

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
                Thread.sleep(1000);
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
        }
    }

}
