import java.util.ArrayList;

public class MatchMonitor extends RegisteredThread{

    private final long _tick = 500;
    private long _inactivityCheckElapsed = 0;
    public MatchMonitor(){
        new RegisteredThread(this).start();
    }
    public void run(){
        Register("MatchMonitor");
        while(!_terminated){
            try {
                CheckForExpiration();
                if(MatchManager.UpdatesNeeded){
                    MatchManager.NotifySubscribers();
                }
                Thread.sleep(_tick);
            }
            catch(InterruptedException ie){
                _terminated = true;
            }
            catch (Exception e) {
                Main.LogError("MatchMonitor.run(): " + e.getMessage());
                Main.LogStackTrace(e);
            }
        }
        Deregister();
        Main.LogMessage("MatchMonitor terminated.");
    }

    private void CheckForExpiration()
    {
        long currentTime = System.currentTimeMillis();
        ArrayList<Match> activeMatches = MatchManager.GetMatches();
        boolean checkPlayerInactivity = false;
        long _inactivityInterval = 30000;
        if(_inactivityCheckElapsed >= _inactivityInterval){
            _inactivityCheckElapsed = 0;
            checkPlayerInactivity = true;
        }
        else{
            _inactivityCheckElapsed += _tick;
        }
        for(Match match : activeMatches){
            if(currentTime >= match.GetExpiration()){
                match.MarkExpired();
                if(match.IsQuickMatch()){
                    MatchManager.AddQuickMatch();
                }
            }
            else{
                match.Tick(_tick);
                if(match.ScoreUpdated()){
                    MatchManager.UpdateScore(match.MatchID(), match.GetScoreBytes());
                }
                if(checkPlayerInactivity){
                    match.CheckForInactivity();
                }
            }
        }
    }
}
