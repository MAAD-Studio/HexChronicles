public interface StateInterface<t>
{
    void EnterState(t manager);

    void UpdateState();

    void ExitState();
}
