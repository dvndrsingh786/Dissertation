using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] float speedOfBullet = 1;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.right * 10, speedOfBullet * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Fire"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
