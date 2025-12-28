using UnityEngine;

public class SkillIndicator : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Material material;
    private float lifeTime;
    private float elapsed;

    public enum IndicatorShape
    {
        Circle,
        Rectangle,
        Cone,
        Ring
    }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        material = new Material(Shader.Find("Sprites/Default"));
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.renderQueue = 3000;
        
        meshRenderer.material = material;
    }

    /// <summary>
    /// Setup indicator với anchor point và aim direction
    /// </summary>
    public void Setup(Vector3 anchorPos, Vector3 aimDirection, IndicatorShape shape, Vector2 size, Color color, float duration)
    {
        anchorPos.z = 0.3f; // ensure indicator is above ground
        transform.position = anchorPos;
        lifeTime = duration;
        elapsed = 0f;
        color.a = 0.5f; 
        material.color = color;

        // Xoay indicator theo aim direction
        ApplyRotation(shape, aimDirection);

        // Tạo mesh theo shape
        meshFilter.mesh = GenerateMesh(shape, size);
    }

    /// <summary>
    /// Áp dụng rotation dựa trên shape và aim direction
    /// </summary>
    private void ApplyRotation(IndicatorShape shape, Vector3 aimDirection)
    {
        if (aimDirection == Vector3.zero)
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        // Normalize aim direction
        aimDirection.Normalize();

        switch (shape)
        {
            case IndicatorShape.Circle:
                // Circle không cần xoay (đối xứng)
                transform.rotation = Quaternion.identity;
                break;

            case IndicatorShape.Rectangle:
                // Rectangle xoay để chiều dài hướng theo aimDirection
                // Anchor ở giữa cạnh, hình chữ nhật mở rộng về phía trước
                float angleRect = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angleRect);
                break;

            case IndicatorShape.Cone:
                // Cone xoay để đỉnh hướng theo aimDirection
                float angleCone = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angleCone - 90f); // -90 để hướng lên
                break;

            case IndicatorShape.Ring:
                // Ring không cần xoay (đối xứng)
                transform.rotation = Quaternion.identity;
                break;
        }
    }

    private void Update()
    {
        elapsed += Time.deltaTime;

        // Fade out effect
        if (elapsed >= lifeTime * 0.7f)
        {
            float fadeProgress = (elapsed - lifeTime * 0.7f) / (lifeTime * 0.3f);
            Color currentColor = material.color;
            currentColor.a = Mathf.Lerp(currentColor.a, 0f, fadeProgress);
            material.color = currentColor;
        }

        // Tự động trả về pool
        if (elapsed >= lifeTime)
        {
            SkillIndicatorPool.Instance?.ReturnIndicator(this);
        }
    }

    private Mesh GenerateMesh(IndicatorShape shape, Vector2 size)
    {
        Mesh mesh = new Mesh();

        switch (shape)
        {
            case IndicatorShape.Circle:
                mesh = GenerateCircle(size.x, 32);
                break;
            case IndicatorShape.Rectangle:
                mesh = GenerateRectangle(size.x, size.y);
                break;
            case IndicatorShape.Cone:
                mesh = GenerateCone(size.x, size.y, 32);
                break;
            case IndicatorShape.Ring:
                mesh = GenerateRing(size.x, size.y, 32);
                break;
        }

        return mesh;
    }

    private Mesh GenerateCircle(float radius, int segments)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];
        Vector2[] uvs = new Vector2[segments + 1];

        // Center anchor
        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );
            uvs[i + 1] = new Vector2(
                0.5f + Mathf.Cos(angle) * 0.5f,
                0.5f + Mathf.Sin(angle) * 0.5f
            );

            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 2) > segments ? 1 : (i + 2);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

    private Mesh GenerateRectangle(float width, float height)
    {
        Mesh mesh = new Mesh();
        
        // Anchor ở giữa cạnh bên trái, mở rộng sang phải
        // X: 0 (anchor) đến width (end)
        // Y: -height/2 đến height/2 (centered)
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, -height/2, 0),        // Bottom-left (anchor)
            new Vector3(width, -height/2, 0),    // Bottom-right
            new Vector3(0, height/2, 0),         // Top-left (anchor)
            new Vector3(width, height/2, 0)      // Top-right
        };

        int[] triangles = new int[6] { 0, 2, 1, 2, 3, 1 };
        
        Vector2[] uvs = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

    private Mesh GenerateCone(float radius, float angle, int segments)
    {
        Mesh mesh = new Mesh();
        
        int halfSegments = Mathf.CeilToInt(segments * (angle / 360f));
        Vector3[] vertices = new Vector3[halfSegments + 2];
        int[] triangles = new int[halfSegments * 3];
        Vector2[] uvs = new Vector2[halfSegments + 2];

        // Anchor ở đỉnh cone
        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0);

        float startAngle = -angle / 2;
        float angleStep = angle / halfSegments;

        for (int i = 0; i <= halfSegments; i++)
        {
            float currentAngle = (startAngle + i * angleStep) * Mathf.Deg2Rad;
            // Cone mở rộng về phía trên (Y dương)
            vertices[i + 1] = new Vector3(
                Mathf.Sin(currentAngle) * radius,
                Mathf.Cos(currentAngle) * radius,
                0f
            );
            uvs[i + 1] = new Vector2((float)i / halfSegments, 1);
        }

        for (int i = 0; i < halfSegments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

    private Mesh GenerateRing(float innerRadius, float outerRadius, int segments)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments * 2];
        int[] triangles = new int[segments * 6];
        Vector2[] uvs = new Vector2[segments * 2];

        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            // Center anchor
            vertices[i * 2] = new Vector3(cos * innerRadius, sin * innerRadius, 0f);
            uvs[i * 2] = new Vector2((float)i / segments, 0);

            vertices[i * 2 + 1] = new Vector3(cos * outerRadius, sin * outerRadius, 0f);
            uvs[i * 2 + 1] = new Vector2((float)i / segments, 1);

            int nextI = (i + 1) % segments;
            
            triangles[i * 6] = i * 2;
            triangles[i * 6 + 1] = i * 2 + 1;
            triangles[i * 6 + 2] = nextI * 2;

            triangles[i * 6 + 3] = nextI * 2;
            triangles[i * 6 + 4] = i * 2 + 1;
            triangles[i * 6 + 5] = nextI * 2 + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

    private void OnDestroy()
    {
        if (material != null)
            Destroy(material);
        
        if (meshFilter != null && meshFilter.mesh != null)
            Destroy(meshFilter.mesh);
    }
}