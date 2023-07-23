using System.Collections.Generic;

namespace CountTo100.Utilities
{
    public abstract class State
    {
        public readonly Enums.State StateEnum;
        public readonly Dictionary<(Enums.State, Enums.State), StateTransition> AvailableStateTransitions;

        protected readonly IStateManageable _stateManager;

        protected State(Enums.State stateEnum, IStateManageable stateManager, IEnumerable<StateTransition> availableStateTransitions)
        {
            StateEnum = stateEnum;
            _stateManager = stateManager;
            AvailableStateTransitions = new Dictionary<(Enums.State, Enums.State), StateTransition>();
            foreach (var transition in availableStateTransitions)
            {
                AvailableStateTransitions.Add((transition.FromState, transition.ToState), transition);
            }
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