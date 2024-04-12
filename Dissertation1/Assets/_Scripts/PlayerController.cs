using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float jumpForce = 1;
    [SerializeField] bool canJump = true;
    int _playerHealth = 100;
    [SerializeField] int PlayerHealth
    {
        get
        {
            return _playerHealth;
        }
        set
        {
            if (value < 0) return;
            _playerHealth = value;
            GameplayManager.instance.SetHealthUI(_playerHealth);
            if (_playerHealth <= 0)
            {
                GameplayManager.instance.GameOver();
                GameManager.instance.GameOver();
            }
        }
    }
    [SerializeField] float shootCooldownPeriod = 1;
    [SerializeField] bool canShoot = true;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        PlayerInput();
    }

    void PlayerInput()
    {
        if (GameManager.gameState != GameState.Gameplay) return;
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            if (transform.localPosition.y < 1.6)
            {
                rb.AddForce(Vector2.up * jumpForce);
            }
            else canJump = false;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (canShoot)
                Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        canShoot = false;
        Invoke(nameof(EnableShooting), shootCooldownPeriod);
    }

    void EnableShooting() => canShoot = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fire"))
        {
            PlayerHealth = 0;
        }
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            PlayerHealth -= collision.gameObject.GetComponent<ObstacleScript>().damage;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!canJump) canJump = true;
        }
    }
}