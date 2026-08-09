import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class TimedObjectCollection<K extends Number, V extends TimedObject> extends ConcurrentHashMap<K, V>{
    private final long _interval;
    private float _elapsed;
    private final ArrayList<V> _expiredObjects;
    private final ArrayList<? super Number> _expiredIDs;
    public TimedObjectCollection(long interval){
        super();
        _interval = interval;
        _elapsed = 0;
        _expiredIDs = new ArrayList<>();
        _expiredObjects = new ArrayList<>();
    }
    private void ClearExpirations(){
        _expiredObjects.clear();
        _expiredIDs.clear();
    }
    public ArrayList<V> GetExpiredObjects(){
        return _expiredObjects;
    }
    public boolean CountdownObjects(long msElapsed){
        _elapsed += msElapsed;
        if(!_expiredObjects.isEmpty()){
            ClearExpirations();
        }
        if(_elapsed >= _interval){
            int intervalsElapsed = (int)Math.floor(_elapsed / _interval);
            _elapsed -= intervalsElapsed * _interval;
            if(!isEmpty()){
                for(V to : values()){
                    if(to.ReduceDuration(intervalsElapsed * _interval)){
                        _expiredObjects.add(to);
                        _expiredIDs.add(to.ObjectID());
                    }
                    //else{
                    //    Main.LogDebug(to.ObjectID() + " duration remaining: " + to.DurationRemaining());
                    //}
                }
                for(TimedObject to: _expiredObjects){
                    remove(to.ObjectID());
                }
            }
        }
        return !_expiredObjects.isEmpty();
    }
    public ArrayList<? super Number> GetExpiredIDs(){
        return _expiredIDs;
    }
    @Override
    public String toString(){
        String toReturn = "";
        for(TimedObject to : values()){
            toReturn = toReturn.concat(to.toString() + System.lineSeparator());
        }
        return toReturn;
    }
}
