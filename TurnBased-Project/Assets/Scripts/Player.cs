using TMPro;
using UnityEngine;

public class Player : MonoBehaviour {

    public string Name => _name;
    public int Score => _score;
    public Color Color => _color;


    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _scoreText;

    private string _name = string.Empty;
    private int _score;
    private Color _color;



    public void Initialize(string name, Color color)
    {
        _name = name;
        _nameText.text = name;

        _color = color;
        SetScore(0);
    }

    public void SetScore(int score)
    {
        _score = score;
        _scoreText.text = _score.ToString();
    }

}
