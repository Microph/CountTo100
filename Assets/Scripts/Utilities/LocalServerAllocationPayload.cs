using System;

[Serializable]
public class LocalServerAllocationPayload
{
    public int numberOfPlayers;

    public LocalServerAllocationPayload(int numberOfPlayers)
    {
        this.numberOfPlayers = numberOfPlayers;
    }
}