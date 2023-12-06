using System.Collections.Generic;

namespace CountTo100.Utilities
{
    public abstract class State<T>
    {
        public readonly Enums.State StateEnum;
        public readonly Dictionary<(Enums.State, Enums.State), StateTransition> AvailableStateTransitions;

        protected IStateManageable<T> _stateManager;
        protected T _context;

        protected State(Enums.State stateEnum, IEnumerable<StateTransition> availableStateTransitions)
        {
            StateEnum = stateEnum;
            AvailableStateTransitions = new Dictionary<(Enums.State, Enums.State), StateTransition>();
            foreach (var transition in availableStateTransitions)
            {
                AvailableStateTransitions.Add((transition.FromState, transition.ToState), transition);
            }
        }

        public virtual void Initialize(IStateManageable<T> stateManager, T context)
        {
            _stateManager = stateManager;
            _context = context;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnFixedUpdate()
        {
        }
    }
}