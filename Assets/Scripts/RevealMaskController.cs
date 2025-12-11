using UnityEngine;

public class RevealMaskController : MonoBehaviour
{
    [Header("Reveal Settings")]
    [Tooltip("遮罩中心（例如 PlayerRoot / 灯 的 Transform）")]
    public Transform source;

    [Tooltip("异世界可见半径")]
    public float radius = 4f;

    [Header("Inner World Materials")]
    [Tooltip("所有使用 InnerWorldReveal Shader 的里世界材质")]
    public Material[] innerMaterials;

    public KeyCode toggleKey = KeyCode.Q;

    private bool isOn = true;
    private float originalRadius;

    // Shader 属性 ID
    private static readonly int RevealCenterID = Shader.PropertyToID("_RevealCenter");
    private static readonly int RevealRadiusID = Shader.PropertyToID("_RevealRadius");

    // ==== 对外公开的静态实例 & 当前状态 ====
    public static RevealMaskController Instance { get; private set; }
    public Vector3 CurrentCenter { get; private set; }
    public float CurrentRadius { get; private set; }

    private void Awake()
    {
        // 简单的单例（场景里只放一个就行）
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple RevealMaskController in scene, destroying duplicate.");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        originalRadius = radius;
    }

    void Update()
    {
        if (source == null || innerMaterials == null || innerMaterials.Length == 0)
            return;

        // Q 键开关灯
        if (Input.GetKeyDown(toggleKey))
        {
            isOn = !isOn;
        }

        float currentRadius = isOn ? originalRadius : 0f;

        Vector3 pos = source.position;
        Vector4 center = new Vector4(pos.x, pos.y, pos.z, 0f);

        // 更新 Shader 材质（视觉上的洞）
        foreach (var mat in innerMaterials)
        {
            if (mat == null) continue;

            mat.SetVector(RevealCenterID, center);
            mat.SetFloat(RevealRadiusID, currentRadius);
        }

        // 更新公开给其他脚本的状态
        CurrentCenter = pos;
        CurrentRadius = currentRadius;
    }

    private void OnDrawGizmosSelected()
    {
        if (source == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(source.position, radius);
    }
}
