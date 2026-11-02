using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, ISavable
{
    public Rigidbody RB;
    public float moveSpeed;

    public event Action OnEncountered;

    private Vector2 moveInput;

    [SerializeField] private string targetTag = "Enemy";

    // This list stores the references so we can find them even when disabled
    private List<GameObject> cachedObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // 1. Find all active objects with the tag at the very beginning
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(targetTag);

        // 2. Add them to our permanent list
        cachedObjects.AddRange(foundObjects);
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
            collision.gameObject.SetActive(false);
        }
    }

    public object CaptureState()
    {
        float[] position = new float[] { transform.position.x, transform.position.y };
        return position;
    }

    public void RestoreState(object state)
    {
        var position = (float[])state;
        transform.position = new Vector3(position[0], position[1]);
        EnableAll();
    }


    // Call this to enable all cached objects (works even if they are inactive!)
    public void EnableAll()
    {
        foreach (GameObject obj in cachedObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }


}
