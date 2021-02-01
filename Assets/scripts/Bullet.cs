using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform target;
    public string targetTag;
    private Vector3 fireDirection;
    public int damage = 50;
    public float speed = 70f;
    private float travelDistance = 0;
    public float maxReachDistance = 20f;
    private Vector3 firedAt;
    public SpriteRenderer spriteToFade;

    public void Seek(Transform _target) {
        target = _target;
        targetTag = target.gameObject.tag;
        firedAt = transform.position;
        fireDirection = (target.position - transform.position).normalized;
        Rigidbody rb = this.GetComponent<Rigidbody>();

        rb.AddForce(fireDirection * speed);
    }

    void Hit()
    {
        Destroy(gameObject);
    }

    private void LateUpdate() {
        Vector3 pos = gameObject.transform.position;
        float dist = Vector3.Distance(pos, firedAt);
        travelDistance = dist;
        // Debug.Log("distance = " + dist);
        if (dist > maxReachDistance) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        GameObject otherObject = other.gameObject;
        string currentObjectTag = gameObject.tag;

        if (otherObject.CompareTag(targetTag)) {
            Hit();
        }
    }
}
