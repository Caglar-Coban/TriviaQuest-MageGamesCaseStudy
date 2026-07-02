// Data/LeaderboardEntry.cs
using System;

[Serializable]
public class LeaderboardEntry
{
    public int rank;
    public string nickname;
    public int score;
}

[Serializable]
public class LeaderboardPage
{
    public int page;
    public bool is_last;
    public LeaderboardEntry[] data;
}