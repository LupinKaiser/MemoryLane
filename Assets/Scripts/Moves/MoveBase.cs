using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Action/Create new action")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string moveName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] EnemyBase.EmoType enemyType;
    [SerializeField] int damage;
 

    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }

    public EnemyBase.EmoType Type {
        get { return enemyType; }
    }

    public int Damage {
        get { return damage; }
    }

}
