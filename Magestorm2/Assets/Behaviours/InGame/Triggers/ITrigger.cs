public interface ITrigger
{
    void EnterAction();
    void ExitAction();
    bool HasEntered();
    bool HasExited();
    int GetTriggerID();
}
