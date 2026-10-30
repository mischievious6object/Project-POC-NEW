using UnityEngine;
public class StartGame : MonoBehaviour
{
    [SerializeField] public GameManager gameManager;
    void Update()
    {
        //detect the X button on the left controller
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            gameManager.StartGame();
            Debug.Log("Start Game");
        }
    }
}
