using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Karakter Prefabları")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    private void Start()
    {
        SpawnCharacters();
    }

    private void SpawnCharacters()
    {
        if (playerPrefab == null)
        {
            Debug.LogError(
                "GameManager içindeki Player Prefab alanı boş."
            );

            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError(
                "GameManager içindeki Enemy Prefab alanı boş."
            );

            return;
        }

        GameObject humanObject = Instantiate(
            playerPrefab,
            Vector3.zero,
            Quaternion.identity
        );

        GameObject enemyObject = Instantiate(
            enemyPrefab,
            Vector3.zero,
            Quaternion.identity
        );

        ConfigureLevelPositions(humanObject, enemyObject);
    }

    private static void ConfigureLevelPositions(
        GameObject humanObject,
        GameObject enemyObject
    )
    {
        GameProgression.GameMode mode =
            (GameProgression.GameMode)PlayerPrefs.GetInt(
                GameProgression.GameModeKey,
                (int)GameProgression.GameMode.Standard
            );

        if (mode != GameProgression.GameMode.Levels)
        {
            return;
        }

        int levelNumber = PlayerPrefs.GetInt(
            GameProgression.SelectedLevelKey,
            1
        );
        LevelDefinition level = LevelCatalog.GetLevel(levelNumber);

        GridMovement human = humanObject.GetComponent<GridMovement>();
        GridMovement enemy = enemyObject.GetComponent<GridMovement>();

        if (human != null)
        {
            human.SetStartingPosition(
                level.HumanStart.X,
                level.HumanStart.Z
            );
        }

        if (enemy != null)
        {
            enemy.SetStartingPosition(
                level.EnemyStart.X,
                level.EnemyStart.Z
            );
        }
    }
}
