using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorToOtherRoom : MonoBehaviour
{
    public Transform player, destination;

    public GameObject playerg;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
           playerg.SetActive(false);
           player.position = destination.position;
           playerg.SetActive(true);

        }
    }
}
