public interface StateInterface<t>
{
    void EnterState(t manager);

    void UpdateState(t manager);

    void ExitState(t manager);
}
