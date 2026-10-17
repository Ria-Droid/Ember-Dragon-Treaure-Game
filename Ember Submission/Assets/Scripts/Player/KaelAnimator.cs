using System;
using UnityEngine;

public class KaelAnimator : MonoBehaviour
{
    public Animator animator;
    public event Action OnFireFrame;

    Vector3 lastPos;
    float speed;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        lastPos = transform.parent.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) PlayDeath();
        Vector3 p = transform.parent.position;
        Vector3 d = p - lastPos;
        d.y = 0;
        float target = d.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        speed = Mathf.Lerp(speed, target, 10f * Time.deltaTime);
        animator.SetFloat("Speed", speed);
        lastPos = p;
    }

    public void PlayShoot() { animator.SetTrigger("Shoot"); }
    public void PlayDeath() {
        Debug.Log("KAEL DEATH FUNCTION CALLED");
        animator.ResetTrigger("Shoot");
        animator.CrossFade("KaelRig|Death", 0.1f);
    }

    // Called by animation events
    public void FireStart() { OnFireFrame?.Invoke(); }
    public void FireEnd() { }
}