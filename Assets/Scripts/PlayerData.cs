public class PlayerData
{
    public ulong ClientId;
    public string PlayerName;

    public PlayerData(ulong clientId, string playerName)
    {
        this.ClientId = clientId;
        this.PlayerName = playerName;
    }
}