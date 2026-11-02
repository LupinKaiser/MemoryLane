using UnityEngine;
using UnityEngine.SceneManagement;


public class ToNextMap : MonoBehaviour
{
   

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadSceneAsync("Test_Room 2");
        }
    }

}
