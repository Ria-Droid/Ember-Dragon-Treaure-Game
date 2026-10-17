using UnityEngine;

public class ArenaBoundary : MonoBehaviour
{
    [Header("Boundary")]
    public Transform center;        // drag CircularArenaGround here
    public float radius = 30f;      // tweak until it sits on the edge of the grass

    private Rigidbody rb;
    private CharacterController cc;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
    }

    void LateUpdate()
    {
        Vector3 c = center != null ? center.position : Vector3.zero;
        Vector3 pos = transform.position;

        Vector3 offset = pos - c;
        offset.y = 0f;

        if (offset.magnitude <= radius) return;

        Vector3 clamped = c + offset.normalized * radius;
        clamped.y = pos.y;   // keep current height

        if (cc != null)
        {
            cc.enabled = false;              // CharacterController blocks direct position changes
            transform.position = clamped;
            cc.enabled = true;
        }
        else if (rb != null)
        {
            rb.position = clamped;

            // cancel any velocity pushing outward
            Vector3 outward = offset.normalized;
            Vector3 v = rb.linearVelocity;
            float outSpeed = Vector3.Dot(new Vector3(v.x, 0f, v.z), outward);
            if (outSpeed > 0f) rb.linearVelocity -= outward * outSpeed;
        }
        else
        {
            transform.position = clamped;
        }
    }

    // draws the boundary circle in the Scene view when the Player is selected
    void OnDrawGizmosSelected()
    {
        Vector3 c = center != null ? center.position : Vector3.zero;
        Gizmos.color = Color.red;
        const int steps = 64;
        Vector3 prev = c + new Vector3(radius, 0f, 0f);
        for (int i = 1; i <= steps; i++)
        {
            float a = i * Mathf.PI * 2f / steps;
            Vector3 next = c + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * radius;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}