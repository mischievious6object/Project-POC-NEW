using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPlayerTurn : PlayerBaseTurn
{
    public override void EnterTurn(PlayerTurnManager turn)
    {
        Debug.Log("PLAYER 1 TURN");
    }
    public override void SwapTurn(PlayerTurnManager turn)
    {

    }
    public override void UpdateState(PlayerTurnManager turn)
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("switched to player 2");
            turn.SwitchTurn(turn.secondPlayer);
        }
    }
    public override void ExtraTurn(PlayerTurnManager turn)
    {

    }


}
