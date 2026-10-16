using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public EnemyBase Base { get; set; }
    public int Level { get; set; }

    public int HP { get; set; }

    public int MaxHP { get; set; }
    public List<Move> Moves { get; set; }

    public Enemy(EnemyBase pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;
        HP = 10;
        MaxHP = 10;


        // Generate Moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            Moves.Add(new Move(move.Base));
        }
    }

    public bool TakeDamage(Move move, Enemy attacker)
    {

        int damage = move.Base.Damage;

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
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