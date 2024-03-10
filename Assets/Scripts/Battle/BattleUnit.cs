using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] CreatureBase _base;
    [SerializeField] int level;
    [SerializeField] public BattleHud Hud;
    public Creature Creature { get; set; }

    public void Setup()
    {
        Creature = new Creature(_base, level);
        GetComponent<Image>().sprite = Creature.Base.Sprite;
    }
}
