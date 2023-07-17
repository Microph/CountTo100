public static class GlobalConfigManager
{
    public static bool IsServer = false;
    public static bool IsClient => !IsServer;
    public static LocalServerAllocationPayload LocalServerAllocationPayload = null; //For local server testing only
}
