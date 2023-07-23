namespace CountTo100.Utilities
{
    public class Delegates
    {
        public delegate void ValueChangedAction<T>(T previousValue, T newValue);
    }
}