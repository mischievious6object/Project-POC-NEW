using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPlayerTurn : PlayerBaseTurn
{
    public void player1() // parameter string, player1 of player2 > player1 kleur groen player 2 rood
    {
        int color;
    }
    public override void EnterTurn(PlayerTurnManager turn)
    {
        //TODO highlight current player in UI
        Debug.Log("PLAYER 1 TURN");
    }
    public override void SwapTurn(PlayerTurnManager turn)
    {

    }
    public override void UpdateState(PlayerTurnManager turn)
    {

        //TODO if line doesnt enclose a box go to player 2, else stay on player 1
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            //TODO turn the UI highlighter for the current player off 
            Debug.Log("switched to player 2");
            turn.SwitchTurn(turn.secondPlayer);
        }
        //TODO else if the player encloses a box call the add point function and forward as parameter the current player
    }



}
