using UnityEngine;
using UnityEngine.SceneManagement;

public class NavManager : MonoBehaviour
{
    public void RestartGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentSceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
