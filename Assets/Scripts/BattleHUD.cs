using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class BattleHUD : MonoBehaviour
{

    [SerializeField] HPBar hpBar;

    BattleUnit hp;

    public void SetData(BattleUnit enemy)
    {
        hp = enemy;
        hpBar.SetHP((float)enemy.Enemy.HP / enemy.Enemy.MaxHP);
    }

    public void UpdateHP()
    {
        hpBar.SetHP((float)hp.Enemy.HP / hp.Enemy.MaxHP);
    }
}

