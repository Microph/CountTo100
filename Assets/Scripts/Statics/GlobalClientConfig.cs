public static class GlobalClientConfig
{
    public static bool IsClient => !GlobalServerConfig.IsServer;
}
