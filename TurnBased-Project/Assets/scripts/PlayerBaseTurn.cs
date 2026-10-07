using Unity.VisualScripting;
using UnityEngine;

public abstract class PlayerBaseTurn
{
    public abstract void EnterTurn(PlayerTurnManager turn);
    public abstract void SwapTurn(PlayerTurnManager turn);
    public abstract void UpdateState(PlayerTurnManager turn);
    public abstract void ExtraTurn(PlayerTurnManager turn);

}
