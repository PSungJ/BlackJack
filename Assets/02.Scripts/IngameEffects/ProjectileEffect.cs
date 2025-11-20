using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEffect : MonoBehaviour
{
    public float speed = 100f;
    private Transform target;
    private System.Action onHit;

    public void Init(Transform target, System.Action onHitCallback)
    {
        this.target = target;
        this.onHit = onHitCallback;
    }

    void Update()
    {
        if (target == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 10f)
        {
            onHit?.Invoke();
            Destroy(gameObject);
        }
    }
}
