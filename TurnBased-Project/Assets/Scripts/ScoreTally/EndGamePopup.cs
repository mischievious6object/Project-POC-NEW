using TMPro;
using UnityEngine;

public class EndGamePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _winText;

    public void Show(Player winner)
    {
        if (winner != null)
        {
            _winText.text = $"{winner.Name} Wins!";
            _winText.color = winner.Color;
        }
        else
        {
            _winText.text = "Tie!";
        }
        gameObject.SetActive(true);
    }

}
