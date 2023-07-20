using UnityEngine;

public class PlayerData
{
    public ulong ClientId;
    public string PlayerName;
    public Color PlayerColor;
    public bool ReadyStatus;
    public int CumulativeClicks;
    public float CumulativeClicksResetTimer;

    public PlayerData(ulong clientId, string playerName, Color playerColor)
    {
        this.ClientId = clientId;
        this.PlayerName = playerName;
        this.ReadyStatus = false;
        this.CumulativeClicks = 0;
        this.CumulativeClicksResetTimer = 0;
        this.PlayerColor = playerColor;
    }
}