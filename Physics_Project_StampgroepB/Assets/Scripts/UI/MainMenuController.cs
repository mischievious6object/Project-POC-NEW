using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] private SettingController _settingController;

    public void Play()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void Levels()
    {
        //SceneManager.LoadScene("Levels");
    }

    public void Settings()
    {
        _settingController.ShowSettings();
    }

    public void Quit()
    {
        Application.Quit();
    }


}
