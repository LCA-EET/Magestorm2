public interface ITimedObject {
    boolean ReduceDuration(long msReduction);
    boolean IsExpired();
    short TimedObjectID();
}
