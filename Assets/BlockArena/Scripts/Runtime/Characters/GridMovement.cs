using System.Collections;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    [Header("Karakter Türü")]
    [SerializeField] private bool isHumanPlayer = true;

    [Header("Tahta Konumu")]
    [SerializeField] private int currentX = 3;
    [SerializeField] private int currentZ = 0;

    [Header("Hareket Ayarları")]
    [SerializeField] private float tileSize = 1.1f;
    [SerializeField] private float playerHeight = 0.55f;
    [SerializeField] private float movementDuration = 0.2f;

    public bool IsHumanPlayer => isHumanPlayer;
    public int CurrentX => currentX;
    public int CurrentZ => currentZ;
    public bool IsMoving { get; private set; }

    private void Start()
    {
        SnapToGrid();
    }

    public void MoveTo(int targetX, int targetZ)
    {
        if (IsMoving)
        {
            return;
        }

        currentX = targetX;
        currentZ = targetZ;

        Vector3 targetPosition = GetWorldPosition(
            currentX,
            currentZ
        );

        StartCoroutine(
            MoveRoutine(targetPosition)
        );
    }

    private IEnumerator MoveRoutine(
        Vector3 targetPosition
    )
    {
        IsMoving = true;

        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

     while (elapsedTime < movementDuration)
{
    elapsedTime += Time.deltaTime;

    float progress =
        elapsedTime / movementDuration;

    Vector3 currentPosition =
        Vector3.Lerp(
            startPosition,
            targetPosition,
            progress
        );

    float jumpHeight =
        Mathf.Sin(progress * Mathf.PI) * 0.30f;

    currentPosition.y += jumpHeight;

    transform.position = currentPosition;

    yield return null;
}

        transform.position = targetPosition;
        IsMoving = false;
        yield break;
    }

    private void SnapToGrid()
    {
        transform.position = GetWorldPosition(
            currentX,
            currentZ
        );
    }

    private Vector3 GetWorldPosition(
        int x,
        int z
    )
    {
        return new Vector3(
            x * tileSize,
            playerHeight,
            z * tileSize
        );
    }
}