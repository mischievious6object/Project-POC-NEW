using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void Play()
    {
        //SceneManager.LoadScene("LastLevel");
    }

    public void Levels()
    {
        //SceneManager.LoadScene("Levels");
    }

    public void Settings()
    {
        //SceneManager.LoadScene("Settings");
    }

    public void Quit()
    {
        Application.Quit();
    }


}
