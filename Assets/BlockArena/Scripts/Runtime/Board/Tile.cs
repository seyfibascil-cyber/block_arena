using UnityEngine;

public class Tile : MonoBehaviour
{
    private Renderer tileRenderer;
    private BoardManager boardManager;

    private Color currentBaseColor;
    private Material tileMaterial;
    private Color normalColor = new Color(0.85f, 0.85f, 0.85f);
    private Color blockedColor = new Color(0.35f, 0.35f, 0.35f);

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
            ConfigureStableMobileMaterial(tileMaterial);
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

    private static void ConfigureStableMobileMaterial(Material material)
    {
        if (material == null)
        {
            return;
        }

        if (material.HasProperty("_Metallic"))
        {
            material.SetFloat("_Metallic", 0f);
        }
        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", 0.12f);
        }
        if (material.HasProperty("_SpecularHighlights"))
        {
            material.SetFloat("_SpecularHighlights", 0f);
        }
        if (material.HasProperty("_EnvironmentReflections"))
        {
            material.SetFloat("_EnvironmentReflections", 0f);
        }
    }

    public void SetTheme(Color normal, Color blocked)
    {
        normalColor = normal;
        blockedColor = blocked;
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
            // Engel, karonun üzerinde zaten görsel olarak belli oluyor.
            // Engelli karoyu koyulaştırmak yerine tahtanın kendi rengini koru.
            currentBaseColor = normalColor;
        }
        else if (IsMovementTarget)
        {
            // Seçilebilir alan görünür kalsın, fakat karakter görselinin önüne
            // geçecek kadar neon ve parlak olmasın.
            currentBaseColor = new Color(0.18f, 0.62f, 0.31f);
        }
        else if (IsObstacleTarget)
        {
            currentBaseColor = new Color(0.72f, 0.28f, 0.23f);
        }
        else
        {
            currentBaseColor = normalColor;
        }

        tileMaterial.color = currentBaseColor;
    }

    private void UpdatePulseEffect()
    {
        if (tileMaterial == null)
        {
            return;
        }

        if (IsMovementTarget || IsObstacleTarget)
        {
            float pulseSpeed = IsMovementTarget ? 4f : 5.5f;
            float pulse =
                (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            float brightness = Mathf.Lerp(0.90f, 1.06f, pulse);

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
        if (Application.isMobilePlatform)
        {
            return;
        }

        if (boardManager != null)
        {
            boardManager.OnTileClicked(this);
        }
    }
}
