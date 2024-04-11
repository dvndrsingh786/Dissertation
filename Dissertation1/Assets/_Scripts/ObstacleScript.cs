using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    [SerializeField] float jumpInterval = 2.5f;
    [SerializeField] Vector2 jumpForceMin = new Vector2(-4, 4);
    [SerializeField] Vector2 jumpForceMax = new Vector2(-8, 8);
    [SerializeField] Rigidbody2D rb;
    public int damage = 10;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating(nameof(Jump), jumpInterval, jumpInterval);
    }

    void Jump()
    {
        Vector2 jumpForce = new Vector2(Random.Range(jumpForceMin.x, jumpForceMax.x), Random.Range(jumpForceMin.y, jumpForceMax.y));
        rb.AddForce(jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
