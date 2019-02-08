using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    [Inject] private PlayerStats _stats;
    [SerializeField] private Text _scoreText;

    private void Start()
    {
        _scoreText.text = _stats.Score.ToString();
    }
}
