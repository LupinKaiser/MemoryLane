using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;


public class Enemy
{
    public EnemyBase Base { get; set; }
    public int Level { get; set; }

    public int HP { get; set; }

    public int Damage { get; set; }

    public int MaxHP { get; set; }

    public EnemyBase.EmoType EmoType { get; set; }

    public List<Move> Moves { get; set; }
    public bool IsFainted { get; internal set; }

    [SerializeField] BattleDialogBox dialogBox;

    public Enemy(BattleDialogBox dialogBox)
    {
        this.dialogBox = dialogBox;
    }

    public Enemy(EnemyBase pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;
        HP = pBase.health;
        MaxHP = pBase.maxHealth;
        Damage = pBase.damage;
        EmoType = pBase.Type;
        IsFainted = false;


        // Generate Moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            Moves.Add(new Move(move.Base));
        }
    }

    public bool TakeDamage(Move move, Enemy attacker)
    {
        int healing = 3;
        

        Debug.Log(attacker.Damage);

        if (move.Base.name == "Attack")
        {
            if (move.Base.Type == EnemyBase.EmoType.Anger && EmoType == EnemyBase.EmoType.Fear)
            {
                //dialogBox.TypeDialog("Super effective");

                Debug.Log("Super Effective1");
               
                HP -= (attacker.Damage * 2);
            }
            
                HP -= attacker.Damage;
            
        }
        if (move.Base.name == "Boost")
        {
            attacker.Damage += 2;
        }
        if (move.Base.name == "Weaken")
        {
            Damage -= 2;
        }
        if(move.Base.name == "Heal")
        {
            attacker.HP += healing;
        }

        Debug.Log(attacker.Damage);

        if (HP <= 0)
        {
            HP = 0;
            IsFainted = true;
            return true;
        }

        return false;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

  

}