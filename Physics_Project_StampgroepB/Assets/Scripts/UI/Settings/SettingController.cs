using UnityEngine;

public class SettingController : MonoBehaviour
{
    [SerializeField] private SettingsPanel _controlsPanel;
    [SerializeField] private SettingsPanel _audioPanel;
    private SettingsPanel _openedPanel;

    public void ShowSettings()
    {
        gameObject.SetActive(true);
        OpenPanel(_controlsPanel);
    }

    public void HideSettings()
    {
        gameObject.SetActive(false);
    }

    public void Controls()
    {
        OpenPanel(_controlsPanel);
    }

    public void Audio()
    {
        OpenPanel(_audioPanel);
    }

    private void OpenPanel(SettingsPanel panel)
    {
        if (_openedPanel != null)
        {
            _openedPanel.Hide();
        }
        _openedPanel = panel;
        _openedPanel.Open();
    }

}
