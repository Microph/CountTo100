using CountTo100.Utilities;
using UnityEngine;
using static GameplayServerStateManager;

public class GameplayServerAllowCountingState : State<GameplayServerContext>
{
    public GameplayServerAllowCountingState() 
        : base(
            stateEnum: Enums.State.GameplayServer_AllowCounting,
            availableStateTransitions: new StateTransition[]
            {
                new EndGameStateTransition()
            },
            stateManager: null
        )
    {
    }

    public class EndGameStateTransition : StateTransition
    {
        public EndGameStateTransition()
            : base(Enums.State.GameplayServer_AllowCounting, Enums.State.GameplayServer_EndGame)
        {
        }
    }

    public override void OnEnter()
    {
        _context.GameplaySceneManager.GameplayServerStateManager.OnPlayerCount += OnPlayerCount;
    }

    public override void OnExit()
    {
        _context.GameplaySceneManager.GameplayServerStateManager.OnPlayerCount -= OnPlayerCount;
    }

    public override void OnUpdate()
    {
        foreach((_, PlayerData playerData) in _context.ConnectedPlayerDataDict)
        {
            playerData.CumulativeClicksResetTimer += Time.deltaTime;
            if(playerData.CumulativeClicksResetTimer >= 1)
            {
                playerData.CumulativeClicksResetTimer -= 1;
                playerData.CumulativeClicks = 0;
            }
        }
    }

    private void OnPlayerCount(ulong clientId)
    {
        if (_context.ConnectedPlayerDataDict[clientId].CumulativeClicks >= 5) 
        {
            Debug.Log($"clientId {clientId} reached 5 clicks/sec limit");
            return;
        }

        _context.ConnectedPlayerDataDict[clientId].CumulativeClicks++;
        _context.GameplaySceneManager.GameplayServerStateManager.NVCurrentScore.Value++;
        _context.GameplaySceneManager.GameplayServerStateManager.NVLatestClickerId.Value = clientId;
        _context.ConnectedPlayerDataDict[clientId].PlayerObject.EmitPlus1ClientRpc();
        if (_context.GameplaySceneManager.GameplayServerStateManager.NVCurrentScore.Value == 100)
        {
            _context.GameplaySceneManager.GameplayServerStateManager.NVWinnerClickerName.Value = _context.GameplaySceneManager.GameplayServerStateManager.GetPlayerName(clientId);
            _context.ConnectedPlayerDataDict[clientId].PlayerObject.ShowWinnerSpriteClientRpc();
            _stateManager.TransitTo(_context.GameplayServerStates.GameplayServerEndGameState, _context);
        }
    }
}