namespace CountTo100.Utilities
{
    public static class Enums
    {
        public enum State
        {
            None,
            //Server
            GameplayServer_ServerStarted,
            GameplayServer_BeginGameplayCountDown,
            GameplayServer_AllowCounting,
            GameplayServer_EndGame,
            //Client
            GameplayClient_ClientStarted,
            GameplayClient_BeginGameplayCountDown,
            GameplayClient_AllowCounting,
            GameplayClient_EndGame,
        }
    }
}