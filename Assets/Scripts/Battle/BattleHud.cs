using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar hpBar;
    [SerializeField] GameObject xpBar;

    Creature _creature;

    public void SetData(Creature creature)
    {
        _creature = creature;
        nameText.text = creature.Base.Name;
        SetLevel();
        hpBar.SetHP((float)creature.HP / creature.MaxHp);

        SetXp();
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float)_creature.HP /  _creature.MaxHp);
    }

    public void SetLevel()
    {
        levelText.text = "Lvl: " + _creature.Level;
    }

    public void SetXp()
    {
        if (xpBar == null) return;

        float normalizedXp = GetNormalizedXp();
        xpBar.transform.localScale = new Vector3(normalizedXp, 1, 1);
    }

    public IEnumerator UpdateXp(bool reset = false)
    {
        if (xpBar == null) yield break;
        if (reset)
            xpBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedXp = GetNormalizedXp();
        yield return xpBar.transform.localScale = new Vector3(normalizedXp, 1, 1);
    }
  
    
    float GetNormalizedXp()
    {
        int currLevelXp = _creature.Base.GetXpForLevel(_creature.Level);
        int nextLevelXp = _creature.Base.GetXpForLevel(_creature.Level + 1);

        float normalizedXp = (float)(_creature.XP - currLevelXp) / (nextLevelXp - currLevelXp);
        return Mathf.Clamp01(normalizedXp);
    }
}
