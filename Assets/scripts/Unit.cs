using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // public float speed = 10f;
    public int health = 100;
    public float accelerationTime = 0.5f;
    public float maxSpeed = 0.5f;
    private Vector2 movement;
    private float timeLeft = 0.5f;

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("OnCollisionEnter: unit Hit!");
        GameObject otherObject = other.gameObject;
        if (otherObject.CompareTag("bullet")) {
            Bullet b = otherObject.GetComponent<Bullet>();
            // TODO: Bulletを参照してそこからダメージ計算を行いたいが、Bulletクラスを取得する方法がわからない
            Debug.Log("hit by bullet" + health);
            Debug.Log("b.targetTag = " + b.targetTag);
            Collider c = gameObject.GetComponent<Collider>();
            string t = gameObject.tag;
            Debug.Log("tag is" + t);
            if (t == b.targetTag) {
                TakeDamage(b.damage);
            }
        }
    }
}
