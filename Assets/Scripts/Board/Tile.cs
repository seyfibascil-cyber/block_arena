using UnityEngine;

public class Tile : MonoBehaviour
{
    private Renderer tileRenderer;
    private BoardManager boardManager;

    private Color currentBaseColor;
    private Material tileMaterial;

    public int X { get; private set; }
    public int Z { get; private set; }

    public bool IsMovementTarget { get; private set; }
    public bool IsObstacleTarget { get; private set; }
    public bool IsBlocked { get; private set; }

    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();

        if (tileRenderer != null)
        {
            tileMaterial = tileRenderer.material;
        }
    }

    private void Update()
    {
        UpdatePulseEffect();
    }

    public void Initialize(BoardManager manager, int x, int z)
    {
        boardManager = manager;

        X = x;
        Z = z;

        gameObject.name = $"Tile_{x}_{z}";

        UpdateColor();
    }

    public void SetMovementTarget(bool isTarget)
    {
        IsMovementTarget = isTarget;

        UpdateColor();
    }

    public void SetObstacleTarget(bool isTarget)
    {
        IsObstacleTarget = isTarget;

        UpdateColor();
    }

    public void SetBlocked(bool isBlocked)
    {
        IsBlocked = isBlocked;

        UpdateColor();
    }

    public void ClearHighlights()
    {
        IsMovementTarget = false;
        IsObstacleTarget = false;

        UpdateColor();
    }

    private void UpdateColor()
    {
        if (tileMaterial == null)
        {
            return;
        }

        if (IsBlocked)
        {
            currentBaseColor = new Color(0.35f, 0.35f, 0.35f);
        }
        else if (IsMovementTarget)
        {
            currentBaseColor = new Color(0.15f, 0.85f, 0.25f);
        }
        else if (IsObstacleTarget)
        {
            currentBaseColor = new Color(0.9f, 0.2f, 0.15f);
        }
        else
        {
            currentBaseColor = new Color(0.85f, 0.85f, 0.85f);
        }

        tileMaterial.color = currentBaseColor;
    }

    private void UpdatePulseEffect()
    {
        if (tileMaterial == null)
        {
            return;
        }

        if (IsMovementTarget)
        {
            float pulse = (Mathf.Sin(Time.time * 4f) + 1f) * 0.5f;
            float brightness = Mathf.Lerp(0.75f, 1.15f, pulse);

            Color pulsingColor = currentBaseColor * brightness;
            pulsingColor.a = 1f;

            tileMaterial.color = pulsingColor;
        }
        else
        {
            tileMaterial.color = currentBaseColor;
        }
    }

    private void OnMouseDown()
    {
        if (boardManager != null)
        {
            boardManager.OnTileClicked(this);
        }
    }
}