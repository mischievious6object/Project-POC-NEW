using UnityEngine;

public class PlayerTurnManager : MonoBehaviour
{
    public PlayerBaseTurn currentTurn;
    public FirstPlayerTurn firstPlayer = new FirstPlayerTurn();
    public SecondPlayerTurn secondPlayer = new SecondPlayerTurn();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTurn = firstPlayer;
        currentTurn.EnterTurn(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentTurn.ChangeState(this);
    }
    public void SwitchTurn(PlayerBaseTurn turn)
    {
        currentTurn = turn;
        currentTurn.ChangeState(this);
    }
}
