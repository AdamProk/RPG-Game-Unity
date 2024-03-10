using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Creature", menuName = "Creature/Create new Creature")]

public class CreatureBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite sprite;

    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int xpYield;


    [SerializeField] List<LearnableMove> learnableMoves;

    public string Name
    {get { return name; }}

    public string Description
    { get { return description; } }

    public Sprite Sprite
    { get { return sprite; }}

    public int MaxHp
    { get { return maxHp; }}

    public int Attack
    { get { return attack; }}

    public int Defense
    { get { return defense; }}

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }

    public int XpYield => xpYield; 

    public int GetXpForLevel(int level)
    {
        return 4 * (level * level * level) / 5;
    }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    { get { return moveBase; }}

    public int Level
    { get { return level; }}    

}

   