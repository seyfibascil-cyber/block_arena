using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Arayüz Elemanları")]
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text resultText;

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void ShowHumanMovement()
    {
        SetTurnText("SENİN SIRAN");
    }

    public void ShowHumanObstacle()
    {
        SetTurnText("ENGEL YERLEŞTİR");
    }

    public void ShowEnemyTurn()
    {
        SetTurnText("RAKİP DÜŞÜNÜYOR...");
    }

    public void ShowHumanWin()
    {
        SetTurnText("KAZANDIN!");

        gameOverPanel.SetActive(true);
        resultText.text = "KAZANDIN!";
    }

    public void ShowEnemyWin()
    {
        SetTurnText("KAYBETTİN!");

        gameOverPanel.SetActive(true);
        resultText.text = "KAYBETTİN!";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void SetTurnText(string message)
    {
        if (turnText == null)
        {
            Debug.LogError(
                "UIManager içindeki Turn Text alanı boş."
            );

            return;
        }

        turnText.text = message;
    }
}