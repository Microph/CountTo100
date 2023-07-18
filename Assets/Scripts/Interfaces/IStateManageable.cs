public interface IStateManageable
{
    public void SetState(State state);
    public void TransitTo(State state);
}
