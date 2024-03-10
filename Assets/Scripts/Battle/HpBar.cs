using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHPSmooth(float newHp)
    {
        float currHp = health.transform.localScale.x;
        float changeAmt = currHp - newHp;

        while(currHp - newHp > Mathf.Epsilon)
        {
            currHp -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(currHp, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3(newHp, 1f);   
    }
}