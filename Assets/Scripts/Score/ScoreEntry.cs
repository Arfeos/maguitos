using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreEntry
{
    public string playerName;
    public int score;
    public bool pacifist;

    public ScoreEntry(string name, int score, bool pacifist)
    {
        playerName = name;
        this.score = score;
        this.pacifist = pacifist;
    }
}
