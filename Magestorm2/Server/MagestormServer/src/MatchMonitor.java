public class MatchMonitor extends Thread{

    public MatchMonitor(){
        new Thread(this).start();
    }
    public void run(){
        while(Main.Running){
            try {
                if(MatchManager.UpdatesNeeded){
                    MatchManager.NotifySubscribers();
                }
                Thread.sleep(1000);
            } catch (InterruptedException e) {
                throw new RuntimeException(e);
            }
        }
    }
}
