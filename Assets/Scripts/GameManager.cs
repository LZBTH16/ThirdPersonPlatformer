using UnityEngine;
using TMPro;  // For TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    private int score;
    [SerializeField] private TextMeshProUGUI scoreText;

    void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to update the score
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    // Update the UI with the current score
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
