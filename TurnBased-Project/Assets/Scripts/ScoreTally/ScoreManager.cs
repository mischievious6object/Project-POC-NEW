using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance {get; private set;}

    public int player1Score {get; private set;}
    public int player2Score {get; private set;}

    /// <summary>
    /// when the game starts, make sure only 1 score manager exists
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            //remove this gameobject, we already have a scoremanager
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    //add points to player X
    public void AddPlayer1Score(int amount)
    {
        player1Score += amount;
    }

    public void AddPlayer2Score(int amount)
    {
        player2Score += amount;
    }

    //remove points to player X
    public void RemovePlayer1Score(int amount)
    {
        player1Score -= amount;
    }

    public void RemovePlayer2Score(int amount)
    {
        player2Score -= amount;
    }

    //reset scores
    public void ResetScores()
    {
        player1Score = 0;
        player2Score = 0;
    }

    //fetch scores
    public int FetchPlayer1Score() { 
        return player1Score;
    }
    public int FetchPlayer2Score() { 
        return player2Score;
    }
}