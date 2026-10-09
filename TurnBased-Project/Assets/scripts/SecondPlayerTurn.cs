using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class SecondPlayerTurn : PlayerBaseTurn
{
    public override void EnterTurn(PlayerTurnManager turn)
    {
        Debug.Log("PLAYER 2 TURN");
    }
    public override void SwapTurn(PlayerTurnManager turn)
    {

    }
    public override void UpdateState(PlayerTurnManager turn)
    {
        //Debug.Log("playerplayer");
        //bool iswKeyPressed = Keyboard.current.rKey.isPressed;
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("switched to player 1");
            turn.SwitchTurn(turn.firstPlayer);

        }

    }



}
