using UnityEngine;

[DisallowMultipleComponent]
public class RevealCollider : MonoBehaviour
{
    [Tooltip("额外放大 / 缩小有效半径，用于微调")]
    public float extraRadius = 0f;

    [Tooltip("是否在洞中才启用 Collider（默认 true）")]
    public bool enableInside = true;

    private Collider[] colliders;

    void Awake()
    {
        // 这个物体及其子物体上的全部 Collider
        colliders = GetComponentsInChildren<Collider>();
        if (colliders.Length == 0)
        {
            Debug.LogWarning($"RevealCollider on {name} has no Collider attached.");
        }
    }

    void Update()
    {
        var controller = RevealMaskController.Instance;
        if (controller == null) return;

        Vector3 center = controller.CurrentCenter;
        float radius = controller.CurrentRadius + extraRadius;

        // 如果灯是关的，CurrentRadius 可能为 0
        if (radius <= 0f)
        {
            SetCollidersEnabled(false);
            return;
        }

        float dist = Vector3.Distance(transform.position, center);

        bool shouldEnable;
        if (enableInside)
        {
            // 在洞里开启，洞外关闭
            shouldEnable = dist <= radius;
        }
        else
        {
            // 反向模式：洞外开启，洞里关闭（备用）
            shouldEnable = dist > radius;
        }

        SetCollidersEnabled(shouldEnable);
    }

    private void SetCollidersEnabled(bool value)
    {
        if (colliders == null) return;
        foreach (var col in colliders)
        {
            if (col == null) continue;
            col.enabled = value;
        }
    }
}
