using System;

[Serializable]
public class LocalServerAllocationPayload
{
    public int numberOfPlayers;
    public string serverBindingIP;
    public ushort serverPort;

    public LocalServerAllocationPayload(int numberOfPlayers, string serverBindingIP, ushort serverPort)
    {
        this.numberOfPlayers = numberOfPlayers;
        this.serverBindingIP = serverBindingIP;
        this.serverPort = serverPort;
    }
}