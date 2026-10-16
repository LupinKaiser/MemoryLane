using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody RB;
    public float moveSpeed;

    public event Action OnEncountered;

    private Vector2 moveInput;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        moveInput.Normalize();

        RB.linearVelocity = new Vector3(moveInput.x * moveSpeed, RB.linearVelocity.y, moveInput.y * moveSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            OnEncountered();
            Destroy(collision.gameObject);
        }
    }


}
