public class PlayerData
{
    public ulong ClientId;
    public string PlayerName;
    public bool ReadyStatus;
    public int CumulativeClicks;
    public float CumulativeClicksResetTimer;

    public PlayerData(ulong clientId, string playerName)
    {
        this.ClientId = clientId;
        this.PlayerName = playerName;
        this.ReadyStatus = false;
        this.CumulativeClicks = 0;
        this.CumulativeClicksResetTimer = 0;
    }
}