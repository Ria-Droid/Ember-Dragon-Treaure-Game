using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public float extraHeight = 0.4f;
    public Vector2 barSize = new Vector2(100f, 12f);
    public float worldScale = 0.012f;

    private EnemyBase enemy;
    private Transform bar;
    private RectTransform fill;
    private Image fillImage;
    private float heightOffset;
    private Camera cam;

    void Start()
    {
        enemy = GetComponent<EnemyBase>();
        cam = Camera.main;

        
        float top = transform.position.y + 2f;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            Bounds b = renderers[0].bounds;
            foreach (Renderer r in renderers) b.Encapsulate(r.bounds);
            top = b.max.y;
        }
        heightOffset = top - transform.position.y + extraHeight;

        BuildBar();
    }

    void BuildBar()
    {
        GameObject root = new GameObject(name + " HealthBar", typeof(RectTransform));
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        bar = root.transform;

        RectTransform rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = barSize;
        rt.localScale = Vector3.one * worldScale;

        MakeImage(root.transform, "Background", new Color(0f, 0f, 0f, 0.7f));
        fillImage = MakeImage(root.transform, "Fill", Color.green);
        fill = fillImage.rectTransform;
    }

    Image MakeImage(Transform parent, string objName, Color color)
    {
        GameObject go = new GameObject(objName, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = color;
        RectTransform r = img.rectTransform;
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
        return img;
    }

    void LateUpdate()
    {
        if (bar == null || enemy == null) return;

        if (enemy.IsDead)
        {
            bar.gameObject.SetActive(false);
            return;
        }

        if (cam == null) cam = Camera.main;

        bar.position = transform.position + Vector3.up * heightOffset;
        if (cam != null) bar.rotation = cam.transform.rotation;

        float pct = Mathf.Clamp01((float)enemy.CurrentHealth / enemy.maxHealth);
        fill.anchorMax = new Vector2(pct, 1f);
        fillImage.color = Color.Lerp(Color.red, Color.green, pct);
    }

    void OnDestroy()
    {
        if (bar != null) Destroy(bar.gameObject);
    }
}