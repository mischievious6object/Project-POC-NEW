using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public void AddScoreToPlayer(Player player, int amount)
    {
        int score = player.Score;
        score += amount;

        player.SetScore(score);
    }

}