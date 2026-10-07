using TMPro;
using UnityEngine;

/* this script is used as a prefab, when all houses has been built, create an instance of this object and it will show the results
 * to remove it, reload the scene, because this is only used at the end
 */

public class WinLose : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI WinText;
    private int player1score = 0;
    private int player2score = 0;
    private int resultState = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //fetch the scores


        //calculate who wins
        if (player1score > player2score) { resultState = 1; }
        else if (player1score < player2score) { resultState = 2; }
        else { resultState = 3; }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (resultState)
        {
            case 0:
                WinText.text = "";
                break;
            case 1:
                WinText.text = "Player 1 Wins!";
                break;
            case 2:
                WinText.text = "Player 2 Wins!";
                break;
            default:
                WinText.text = "Tie!";
                break;

        }
    }
}
