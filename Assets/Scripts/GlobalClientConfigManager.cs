public static class GlobalClientConfigManager
{
    public static bool IsClient => !GlobalServerConfigManager.IsServer;
}
