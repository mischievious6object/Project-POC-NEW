using System.Security.Cryptography;
using UnityEngine;

public class PlayerTurnManager : MonoBehaviour
{

    Player player1 = new Player();
    Player player2 = new Player();
    Player currentPlayer = new Player();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPlayer = player1;
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void LineIsDrawn(int pDrawnSquares)
    {
        if (pDrawnSquares >= 0)
        {
            currentPlayer.score++;
        }
        else
        {
            currentPlayer = player2;
        }
    }
}
