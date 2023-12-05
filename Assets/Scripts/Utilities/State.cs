using System.Collections.Generic;

namespace CountTo100.Utilities
{
    public abstract class State
    {
        public readonly Enums.State StateEnum;
        public readonly Dictionary<(Enums.State, Enums.State), StateTransition> AvailableStateTransitions;

        protected IStateManageable _stateManager;

        protected State(Enums.State stateEnum, IEnumerable<StateTransition> availableStateTransitions, IStateManageable stateManager)
        {
            StateEnum = stateEnum;
            AvailableStateTransitions = new Dictionary<(Enums.State, Enums.State), StateTransition>();
            _stateManager = stateManager;
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