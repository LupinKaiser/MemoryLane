using System.Collections.Generic;
using Unity.ProjectAuditor.Editor;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBase", menuName = "Scriptable Objects/EnemyBase")]
public class EnemyBase : ScriptableObject
{
   
    [SerializeField] public Sprite Sprite;

    [SerializeField] EmoType type;

    public new string name;
    [SerializeField] public int Level;

    [SerializeField] public int health;
    [SerializeField] public int maxHealth;

    int Love;
    int Joy;
    int Sadness;
    int Anger;
    int Suprise;
    int Fear;

    [SerializeField] List<LearnableMove> learnableMoves;

    // Start is called before the first frame update
    void Start()
    {
        Love = Random.Range(1, 6);
        Joy = Random.Range(1, 6);
        Sadness = Random.Range(1, 6);
        Anger = Random.Range(1, 6);
        Suprise = Random.Range(1, 6);
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

    public enum EmoType
    {
        Anger,
        Sadness,
        Love,
        Joy,
        Fear,
        Suprise
    }

    public string Name
    {
        get { return name; }
    }

    public Sprite BackSprite
    {
        get { return Sprite; }
    }

    public int MaxHp
    {
        get { return maxHealth; }
    }

    public EmoType Type
    {
        get { return type; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }



    [System.Serializable]
    public class LearnableMove
    {
        [SerializeField] MoveBase moveBase;
        [SerializeField] int level;

        public MoveBase Base
        {
            get { return moveBase; }
        }

        public int Level
        {
            get { return level; }
        }
    }
}
