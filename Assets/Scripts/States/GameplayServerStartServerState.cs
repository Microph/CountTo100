using CountTo100.Utilities;
using Unity.Netcode;

public class GameplayServerStartServerState : State
{
    public GameplayServerStartServerState()
        : base(
            stateEnum: Enums.State.GameplayServer_StartServer,
            availableStateTransitions: new StateTransition[]
            {
                new BeginGameplayCountDownStateTransition()
            }
        )
    {
    }

    public override void OnEnter()
    {
        //TODO inject dependency
        //_networkManager.ConnectionApprovalCallback = ConnectionApprovalCheck;
        //_networkManager.OnClientConnectedCallback += OnClientConnected;
        //_networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        //_transport.SetConnectionData("127.0.0.1", 7777); //TODO: not hardcoded
        //_networkManager.StartServer();
    }

    public class BeginGameplayCountDownStateTransition : StateTransition
    {
        public BeginGameplayCountDownStateTransition() 
            : base(Enums.State.GameplayServer_StartServer, Enums.State.GameplayServer_BeginGameplayCountDown)
        {
        }
    }
}
