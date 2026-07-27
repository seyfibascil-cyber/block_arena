using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum TurnOwner
    {
        Human,
        Enemy
    }

    public enum TurnPhase
    {
        Movement,
        ObstaclePlacement
    }

    [Header("Güncel Tur Bilgisi")]
    [SerializeField] private TurnOwner currentTurnOwner;
    [SerializeField] private TurnPhase currentTurnPhase;

    public TurnOwner CurrentTurnOwner => currentTurnOwner;
    public TurnPhase CurrentTurnPhase => currentTurnPhase;

    private void Start()
    {
        StartHumanTurn();
    }

    public void StartHumanTurn()
    {
        currentTurnOwner = TurnOwner.Human;
        currentTurnPhase = TurnPhase.Movement;

        Debug.Log("İnsan oyuncunun hareket sırası.");
    }

    public void StartHumanObstaclePhase()
    {
        currentTurnOwner = TurnOwner.Human;
        currentTurnPhase = TurnPhase.ObstaclePlacement;

        Debug.Log("İnsan oyuncunun engel yerleştirme sırası.");
    }

    public void StartEnemyTurn()
    {
        currentTurnOwner = TurnOwner.Enemy;
        currentTurnPhase = TurnPhase.Movement;

        Debug.Log("Rakibin hareket sırası.");
    }

    public void StartEnemyObstaclePhase()
    {
        currentTurnOwner = TurnOwner.Enemy;
        currentTurnPhase = TurnPhase.ObstaclePlacement;

        Debug.Log("Rakibin engel yerleştirme sırası.");
    }
}