using System.Collections.Generic;
using UnityEngine;

public interface IScoreService
{
    
    public int GetPoints(string player);
    public void removePoints(string player, int points);
    public void addPoints(string player, int points);
    public void resetScore();
}
