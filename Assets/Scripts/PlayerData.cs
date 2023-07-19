public class PlayerData
{
    public ulong ClientId;
    public string PlayerName;
    public bool ReadyStatus;

    public PlayerData(ulong clientId, string playerName)
    {
        this.ClientId = clientId;
        this.PlayerName = playerName;
        this.ReadyStatus = false;
    }
}