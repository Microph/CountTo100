using System;

[Serializable]
public class LocalServerAllocationPayload
{
    public int numberOfPlayers;
    public ushort serverPort;

    public LocalServerAllocationPayload(int numberOfPlayers, ushort serverPort)
    {
        this.numberOfPlayers = numberOfPlayers;
        this.serverPort = serverPort;
    }
}