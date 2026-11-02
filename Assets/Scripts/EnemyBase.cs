using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBase", menuName = "Scriptable Objects/EnemyBase")]
public class EnemyBase : ScriptableObject
{
   
    [SerializeField] public Sprite Sprite;

    [SerializeField] EmoType type;

    public string Unitname;
    [SerializeField] public int Level;

    [SerializeField] public int health;
    [SerializeField] public int maxHealth;
    [SerializeField] public int damage;

    

    [SerializeField] List<LearnableMove> learnableMoves;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public enum EmoType
    {
        Anger,
        Sadness,
        Love,
        Joy,
        Fear,
        Suprise,
        None
    }

    public string Name
    {
        get { return Unitname; }
    }

    public Sprite BackSprite
    {
        get { return Sprite; }
    }

    public int MaxHp
    {
        get { return maxHealth; }
    }

    public int Damage
    {
        get { return damage; }
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
        [SerializeField] EmoType moveType;
        [SerializeField] int level;

        public MoveBase Base
        {
            get { return moveBase; }
        }

        public int Level
        {
            get { return level; }
        }

        public EmoType EmoType 
        { 
            get { return moveType; }
        }

    }
}
