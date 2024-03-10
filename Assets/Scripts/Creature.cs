using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Creature
{
    public CreatureBase Base { get; set; }
    public int Level { get; set; }
    public int HP { get; set; }
    public int XP { get; set; }

    public List<Move> Moves { get; set; }

    public Creature(CreatureBase cbase, int clevel)
    {
        Base = cbase;
        Level = clevel;
        HP = MaxHp;


        Moves = new List<Move>();
        foreach(var move in Base.LearnableMoves)
        {
            if(move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }

            if (Moves.Count >= 3)
                break;
        }

        XP = Base.GetXpForLevel(Level);
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; }
    }
    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; }
    }
    public int MaxHp
    {
        get { return Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10; }
    }

    public bool TakeDamage(Move move, Creature attacker)
    {
        float modifiers = UnityEngine.Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if(HP <= 0 )
        {
            HP = 0;
            return true;
        }
        return false;
    }

    public Move GetMove()
    {
        int r = UnityEngine.Random.Range(0, Moves.Count);
        return Moves[r];
    }

    public bool CheckForLevelUp()
    {
        int nextLevelXp = Base.GetXpForLevel(Level + 1);
        if (XP > nextLevelXp)
        {
            ++Level;
            return true;
        }
        return false;
    }
}
