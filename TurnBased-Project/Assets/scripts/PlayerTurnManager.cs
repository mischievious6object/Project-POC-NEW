using UnityEngine;

public class PlayerTurnManager : MonoBehaviour
{
    PlayerBaseTurn currentTurn;
    FirstPlayerTurn firstPlayer = new FirstPlayerTurn();
    SecondPlayerTurn secondPlayer = new SecondPlayerTurn();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTurn = firstPlayer;
        currentTurn.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentTurn.ChangeState(this);
    }
    void SwitchTurn(PlayerBaseTurn turn)
    {
        currentTurn = turn;
        currentTurn.ChangeState(this);
    }
}
