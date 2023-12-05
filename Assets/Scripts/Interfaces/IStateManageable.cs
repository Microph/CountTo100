using CountTo100.Utilities;

public interface IStateManageable<T>
{
    public void SetState(State<T> state, T context);
    public void TransitTo(State<T> state, T context);
}
