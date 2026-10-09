using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public void AddScoreToPlayer(Player player, int amount)
    {
        int score = player.Score;
        score += amount;

        player.SetScore(score);
    }

    public Player GetWinner(Player firstPlayer, Player secondPlayer)
    {
        if (firstPlayer.Score > secondPlayer.Score)
        {
            return firstPlayer;
        }
        else if (firstPlayer.Score < secondPlayer.Score)
        {
            return secondPlayer;
        }

        return null;
    }

}