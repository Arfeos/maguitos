using System.Collections.Generic;
using UnityEngine;

public class ScoreService : IScoreService
{
    public Dictionary<string, int> PlayerList = new Dictionary<string, int>();

    public void addPoints(string player, int points)
    {
        if (PlayerList.TryGetValue(player, out int playerPoints))
        {
            playerPoints += points;
            PlayerList[player] = playerPoints;
            Debug.Log("Player: " + player + " has now " + playerPoints + " points.");
        }
        else
        {
            PlayerList.Add(player, points);
            Debug.Log("Player: " + player + " has now " + playerPoints + " points.");
        }
    }

    public int GetPoints(string player)
    {
        if (PlayerList.TryGetValue(player, out int playerPoints)) return playerPoints;
        else
        {
            PlayerList.Add(player, 0);
            return 0;
        }
            
    }

    public void removePoints(string player, int points)
    {
        if (PlayerList.TryGetValue(player, out int playerPoints))
        {
            playerPoints -= points;
            PlayerList[player] = playerPoints;
        }
        else
        {
            PlayerList.Add(player, -points);
        }
    }

    public void resetScore()
    {
        PlayerList = new Dictionary<string, int>();
    }

    public void getScores()
    {

    }
}
