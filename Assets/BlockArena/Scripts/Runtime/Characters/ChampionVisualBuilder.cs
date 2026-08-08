using UnityEngine;

public static class ChampionVisualBuilder
{
    private const string VisualRootName = "ChampionVisual";

    public static void BuildCharacter(GameObject target, ChampionTheme theme, bool enemy)
    {
        if (target == null || theme == null)
        {
            return;
        }

        string resourcePath = GetCharacterResource(theme.Id, !enemy);
        if (string.IsNullOrEmpty(resourcePath))
        {
            return;
        }

        Texture2D texture = Resources.Load<Texture2D>(resourcePath);
        if (texture == null)
        {
            Debug.LogWarning("Champion sprite bulunamadı: " + resourcePath);
            return;
        }

        HideExistingVisuals(target);
        CreateSquareBase(
            target.transform,
            enemy ? new Color(0.78f, 0.12f, 0.18f) : theme.PrimaryColor,
            "ChampionBase"
        );
        CreateBillboard(target.transform, texture, 1.48f, 0.12f, 20, VisualRootName);
    }

    public static void BuildObstacle(GameObject target, ChampionTheme theme)
    {
        if (target == null || theme == null)
        {
            return;
        }

        RemoveGeneratedObstacleVisuals(target.transform);
        target.transform.rotation = Quaternion.identity;

        string resourcePath = GetObstacleResource(theme.ObstacleStyle);
        Texture2D texture = Resources.Load<Texture2D>(resourcePath);
        if (texture == null)
        {
            Debug.LogWarning("Engel sprite'ı bulunamadı: " + resourcePath);
            return;
        }

        foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        target.transform.localScale = Vector3.one;
        CreateBillboard(
            target.transform,
            texture,
            GetObstacleHeight(theme.ObstacleStyle),
            -0.50f,
            10,
            "ObstacleVisual",
            true
        );
    }

    private static void RemoveGeneratedObstacleVisuals(Transform parent)
    {
        string[] generatedNames = { "ObstacleVisual", "ObstacleBase", "ObstacleTop" };
        foreach (string generatedName in generatedNames)
        {
            Transform generated = parent.Find(generatedName);
            if (generated != null)
            {
                Object.Destroy(generated.gameObject);
            }
        }
    }

    private static void CreateObstacleTop(Transform parent, ChampionTheme theme)
    {
        GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
        top.name = "ObstacleTop";
        top.transform.SetParent(parent, false);
        top.transform.localPosition = new Vector3(0f, 0.51f, 0f);
        top.transform.localScale = new Vector3(0.88f, 0.08f, 0.88f);

        Collider topCollider = top.GetComponent<Collider>();
        if (topCollider != null)
        {
            topCollider.enabled = false;
            Object.Destroy(topCollider);
        }

        Renderer topRenderer = top.GetComponent<Renderer>();
        if (topRenderer != null)
        {
            ApplyObstacleMaterial(topRenderer, theme, true);
        }
    }

    private static void ApplyObstacleMaterial(
        Renderer renderer,
        ChampionTheme theme,
        bool accent
    )
    {
        Color mainColor;
        Color accentColor;
        float metallic;
        float smoothness;

        switch (theme.ObstacleStyle)
        {
            case ChampionObstacleStyle.NinjaStar:
                mainColor = new Color(0.08f, 0.11f, 0.24f);
                accentColor = new Color(0.42f, 0.20f, 0.82f);
                metallic = 0.65f;
                smoothness = 0.48f;
                break;
            case ChampionObstacleStyle.ShipWheel:
                mainColor = new Color(0.38f, 0.16f, 0.055f);
                accentColor = new Color(0.92f, 0.52f, 0.08f);
                metallic = 0.12f;
                smoothness = 0.30f;
                break;
            case ChampionObstacleStyle.MoonRock:
                mainColor = new Color(0.66f, 0.69f, 0.76f);
                accentColor = new Color(0.82f, 0.88f, 0.96f);
                metallic = 0.05f;
                smoothness = 0.18f;
                break;
            case ChampionObstacleStyle.EnergyBarrier:
                mainColor = new Color(0.055f, 0.24f, 0.62f);
                accentColor = new Color(0.10f, 0.92f, 1f);
                metallic = 0.72f;
                smoothness = 0.62f;
                break;
            case ChampionObstacleStyle.RuneCrystal:
                mainColor = new Color(0.30f, 0.10f, 0.55f);
                accentColor = new Color(0.72f, 0.25f, 1f);
                metallic = 0.35f;
                smoothness = 0.68f;
                break;
            case ChampionObstacleStyle.DinosaurEgg:
                mainColor = new Color(0.28f, 0.48f, 0.16f);
                accentColor = new Color(0.94f, 0.60f, 0.16f);
                metallic = 0.03f;
                smoothness = 0.20f;
                break;
            case ChampionObstacleStyle.HoneyBarrel:
                mainColor = new Color(0.48f, 0.20f, 0.045f);
                accentColor = new Color(1f, 0.56f, 0.055f);
                metallic = 0.18f;
                smoothness = 0.42f;
                break;
            default:
                mainColor = new Color(0.16f, 0.18f, 0.22f);
                accentColor = new Color(0.34f, 0.38f, 0.44f);
                metallic = 0.18f;
                smoothness = 0.24f;
                break;
        }

        Material material = renderer.material;
        material.color = accent ? accentColor : mainColor;
        if (material.HasProperty("_Metallic"))
        {
            material.SetFloat("_Metallic", metallic);
        }
        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", smoothness);
        }
    }

    private static string GetCharacterResource(ChampionId id, bool rearView)
    {
        switch (id)
        {
            case ChampionId.Classic:
                return rearView
                    ? "BlockArena/Champions/ClassicBlock-Back-v1"
                    : "BlockArena/Champions/ClassicBlock-v1";
            case ChampionId.Ninja:
                return rearView
                    ? "BlockArena/Champions/Ninja-Back-v1"
                    : "BlockArena/Champions/Ninja-v1";
            case ChampionId.Pirate:
                return rearView
                    ? "BlockArena/Champions/Pirate-Back-v1"
                    : "BlockArena/Champions/Pirate-v1";
            case ChampionId.Astronaut:
                return rearView
                    ? "BlockArena/Champions/Astronaut-Back-v1"
                    : "BlockArena/Champions/Astronaut-v1";
            case ChampionId.Robot:
                return rearView
                    ? "BlockArena/Champions/Robot-Back-v1"
                    : "BlockArena/Champions/Robot-v1";
            case ChampionId.Wizard:
                return rearView
                    ? "BlockArena/Champions/Wizard-Back-v1"
                    : "BlockArena/Champions/Wizard-v1";
            case ChampionId.Dinosaur:
                return rearView
                    ? "BlockArena/Champions/Dinosaur-Back-v1"
                    : "BlockArena/Champions/Dinosaur-v1";
            case ChampionId.Bear:
                return rearView
                    ? "BlockArena/Champions/Bear-Back-v1"
                    : "BlockArena/Champions/Bear-v1";
            default:
                return null;
        }
    }

    private static string GetObstacleResource(ChampionObstacleStyle style)
    {
        switch (style)
        {
            case ChampionObstacleStyle.StoneBlock:
                return "BlockArena/Champions/StoneWall-Trimmed-v3";
            case ChampionObstacleStyle.NinjaStar:
                return "BlockArena/Champions/NinjaStar-Trimmed-v3";
            case ChampionObstacleStyle.ShipWheel:
                return "BlockArena/Champions/ShipWheel-Trimmed-v3";
            case ChampionObstacleStyle.MoonRock:
                return "BlockArena/Champions/MoonRock-Trimmed-v3";
            case ChampionObstacleStyle.EnergyBarrier:
                return "BlockArena/Champions/EnergyBarrier-Trimmed-v3";
            case ChampionObstacleStyle.RuneCrystal:
                return "BlockArena/Champions/RuneCrystal-Trimmed-v3";
            case ChampionObstacleStyle.DinosaurEgg:
                return "BlockArena/Champions/DinosaurEgg-Trimmed-v3";
            case ChampionObstacleStyle.HoneyBarrel:
                return "BlockArena/Champions/HoneyBarrel-Trimmed-v3";
            default:
                return null;
        }
    }

    private static float GetObstacleHeight(ChampionObstacleStyle style)
    {
        switch (style)
        {
            case ChampionObstacleStyle.EnergyBarrier:
                return 0.92f;
            case ChampionObstacleStyle.StoneBlock:
                return 0.80f;
            case ChampionObstacleStyle.MoonRock:
                return 0.82f;
            case ChampionObstacleStyle.HoneyBarrel:
                return 0.86f;
            case ChampionObstacleStyle.ShipWheel:
            case ChampionObstacleStyle.DinosaurEgg:
                return 0.96f;
            default:
                return 1.05f;
        }
    }

    private static void HideExistingVisuals(GameObject target)
    {
        Transform oldRoot = target.transform.Find(VisualRootName);
        if (oldRoot != null)
        {
            Object.Destroy(oldRoot.gameObject);
        }

        foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
    }

    private static void CreateBillboard(
        Transform parent,
        Texture2D texture,
        float worldHeight,
        float centerY,
        int sortingOrder,
        string objectName,
        bool bottomAnchored = false
    )
    {
        GameObject visual = new GameObject(objectName);
        visual.transform.SetParent(parent, false);
        visual.transform.localPosition = new Vector3(0f, centerY, 0f);

        float pixelsPerUnit = texture.height / worldHeight;
        if (bottomAnchored)
        {
            // Geniş engeller de karonun dışına taşmasın.
            pixelsPerUnit = Mathf.Max(pixelsPerUnit, texture.width / 0.78f);
        }

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            bottomAnchored ? new Vector2(0.5f, 0f) : new Vector2(0.5f, 0.5f),
            pixelsPerUnit,
            0,
            SpriteMeshType.FullRect
        );
        sprite.name = texture.name + " Runtime Sprite";

        SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = sortingOrder;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        ChampionBillboard billboard = visual.AddComponent<ChampionBillboard>();
        billboard.Initialize(parent, centerY, bottomAnchored);
    }

    private static void CreateSquareBase(
        Transform parent,
        Color color,
        string objectName
    )
    {
        GameObject squareBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
        squareBase.name = objectName;
        squareBase.transform.SetParent(parent, false);

        Vector3 parentScale = parent.lossyScale;
        float scaleX = Mathf.Max(0.01f, Mathf.Abs(parentScale.x));
        float scaleY = Mathf.Max(0.01f, Mathf.Abs(parentScale.y));
        float scaleZ = Mathf.Max(0.01f, Mathf.Abs(parentScale.z));
        squareBase.transform.localScale = new Vector3(
            0.84f / scaleX,
            0.07f / scaleY,
            0.84f / scaleZ
        );
        squareBase.transform.localPosition = new Vector3(
            0f,
            -0.5f / scaleY,
            0f
        );

        Collider baseCollider = squareBase.GetComponent<Collider>();
        if (baseCollider != null)
        {
            baseCollider.enabled = false;
            Object.Destroy(baseCollider);
        }

        Renderer renderer = squareBase.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material stableMobileMaterial = Resources.Load<Material>(
                "BlockArena/Materials/CharacterBase"
            );
            if (stableMobileMaterial != null)
            {
                renderer.material = new Material(stableMobileMaterial);
            }
            renderer.material.color = color;
        }

        BoardAlignedBase alignedBase = squareBase.AddComponent<BoardAlignedBase>();
        alignedBase.Initialize(parent, -0.5f);
    }
}

internal sealed class ChampionBillboard : MonoBehaviour
{
    private Camera targetCamera;
    private Transform anchor;
    private float worldYOffset;
    private bool fitInsideTile;
    private SpriteRenderer spriteRenderer;

    public void Initialize(
        Transform worldAnchor,
        float yOffset,
        bool fitToTile = false
    )
    {
        anchor = worldAnchor;
        worldYOffset = yOffset;
        fitInsideTile = fitToTile;
        spriteRenderer = GetComponent<SpriteRenderer>();
        SnapToAnchor();
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            return;
        }

        SnapToAnchor();

        // Tüm sprite'lar aynı kamera düzleminde kalmalı. Kameranın noktasına
        // tek tek bakarlarsa tahta kenarlarında sağa/sola yatık görünürler.
        transform.rotation = Quaternion.LookRotation(
            -targetCamera.transform.forward,
            targetCamera.transform.up
        );

        if (fitInsideTile)
        {
            FitToTileOnScreen();
        }
    }

    private void FitToTileOnScreen()
    {
        if (anchor == null || spriteRenderer == null || spriteRenderer.sprite == null)
        {
            return;
        }

        const float tileHalfSize = 0.48f;
        const float cameraDepthOffset = 0.60f;
        float boardY = anchor.position.y - 0.50f;
        Vector3 center = new Vector3(anchor.position.x, boardY, anchor.position.z);

        Vector3[] tileWorldCorners =
        {
            center + new Vector3(-tileHalfSize, 0f, -tileHalfSize),
            center + new Vector3(tileHalfSize, 0f, -tileHalfSize),
            center + new Vector3(tileHalfSize, 0f, tileHalfSize),
            center + new Vector3(-tileHalfSize, 0f, tileHalfSize)
        };

        Vector3[] tileScreenCorners = new Vector3[4];
        for (int index = 0; index < tileWorldCorners.Length; index++)
        {
            tileScreenCorners[index] = targetCamera.WorldToScreenPoint(tileWorldCorners[index]);
        }

        Vector2 tileScreenCenter = Vector2.zero;
        float minTileY = float.PositiveInfinity;
        float maxTileY = float.NegativeInfinity;
        foreach (Vector3 corner in tileScreenCorners)
        {
            tileScreenCenter += new Vector2(corner.x, corner.y);
            minTileY = Mathf.Min(minTileY, corner.y);
            maxTileY = Mathf.Max(maxTileY, corner.y);
        }
        tileScreenCenter /= tileScreenCorners.Length;

        // Engel yatayda ortada, dikeyde karonun alt bölümünde dursun.
        // Boyuta dokunmadan yalnızca ekrandaki yerleşimi aşağı alır.
        tileScreenCenter.y = Mathf.Lerp(minTileY, maxTileY, 0.22f);

        // Pivot, engelin alt-orta noktasıdır. Onu karonun görsel merkezine koy.
        // Boyutu değiştirme; yalnız düzlemi kameraya yaklaştırarak tahtanın
        // sprite'ı örtmesini engelle.
        Vector3 tileCenterDepth = targetCamera.WorldToScreenPoint(center);
        tileCenterDepth.x = tileScreenCenter.x;
        tileCenterDepth.y = tileScreenCenter.y;
        tileCenterDepth.z = Mathf.Max(
            targetCamera.nearClipPlane + 0.10f,
            tileCenterDepth.z - cameraDepthOffset
        );
        transform.position = targetCamera.ScreenToWorldPoint(tileCenterDepth);
        transform.localScale = Vector3.one;
    }

    private void SnapToAnchor()
    {
        if (anchor == null)
        {
            return;
        }

        Vector3 anchorPosition = anchor.position;
        transform.position = new Vector3(
            anchorPosition.x,
            anchorPosition.y + worldYOffset,
            anchorPosition.z
        );
    }
}

internal sealed class BoardAlignedBase : MonoBehaviour
{
    private Transform anchor;
    private float worldYOffset;

    public void Initialize(Transform worldAnchor, float yOffset)
    {
        anchor = worldAnchor;
        worldYOffset = yOffset;
        SnapToAnchor();
    }

    private void LateUpdate()
    {
        SnapToAnchor();
        transform.rotation = Quaternion.identity;
    }

    private void SnapToAnchor()
    {
        if (anchor == null)
        {
            return;
        }

        Vector3 anchorPosition = anchor.position;
        transform.position = new Vector3(
            anchorPosition.x,
            anchorPosition.y + worldYOffset,
            anchorPosition.z
        );
    }
}
