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

        Instantiate(
            playerPrefab,
            Vector3.zero,
            Quaternion.identity
        );

        Instantiate(
            enemyPrefab,
            Vector3.zero,
            Quaternion.identity
        );
    }
}