using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmotionStats", menuName = "Player/create player stats")]
public class EmotionStats : ScriptableObject
{

    [SerializeField] Sprite frontSprite;

    

    [SerializeField] public string name;
    [SerializeField] public int Level;

    [SerializeField] public int health;
    [SerializeField] public int maxHealth;

    int Love = Random.Range(1, 6);
    int Joy = Random.Range(1, 6);
    int Sadness = Random.Range(1, 6);
    int Anger = Random.Range(1, 6);
    int Suprise = Random.Range(1, 6);
    int Fear = Random.Range(1, 6);

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public bool TakeDamage()
    {
        int damage = Anger;

        health -= damage;

        if (health <= 0)
            return true;
        else
            return false;
    }

   

}
