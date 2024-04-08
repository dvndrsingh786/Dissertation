using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float jumpForce = 1;
    int _playerHealth = 100;
    [SerializeField] int playerHealth
    {
        get
        {
            return _playerHealth;
        }
        set
        {
            _playerHealth = value;
            if (_playerHealth <= 0)
            {
                GameplayManager.instance.GameOver();
                GameManager.instance.GameOver();
            }
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (transform.localPosition.y < 1.6)
        {
            PlayerInput();
        }
    }

    void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<FireScript>())
        {
            playerHealth = 0;
        }
    }
}