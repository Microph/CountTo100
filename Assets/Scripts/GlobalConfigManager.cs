using CountTo100.Utilities;

public static class GlobalConfigManager
{
    public static bool IsServer = false;
    public static bool IsClient => !IsServer;
}
