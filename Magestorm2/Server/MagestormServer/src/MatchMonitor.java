import java.util.ArrayList;

public class MatchMonitor extends RegisteredThread{

    private final long _tick = 500;
    private TimedObjectCollection<Byte, Match> _activeMatches;
    public MatchMonitor(){
        new RegisteredThread(this).start();
    }
    public void run(){
        Register("MatchMonitor");
        _activeMatches = MatchManager.GetMatches();
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
        if(_activeMatches.CountdownObjects(_tick)){
            ArrayList<Match> expiredMatches = _activeMatches.GetExpiredObjects();
            for(Match expired : expiredMatches){
                expired.MarkExpired();
                if(expired.IsQuickMatch()){
                    MatchManager.AddQuickMatch();
                }
            }
        }
    }
}
