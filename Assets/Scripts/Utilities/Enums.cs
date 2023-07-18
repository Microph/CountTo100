namespace CountTo100.Utilities
{
    public static class Enums
    {
        public enum State
        {
            None,
            //Server
            GameplayServer_StartServer,
            GameplayServer_BeginGameplayCountDown,
            GameplayServer_AllowCounting,
            GameplayServer_GameOver,
            //Client
            GameplayClient_StartClient,
        }
    }
}