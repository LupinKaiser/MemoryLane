using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

    public Slider hpSlider;

    public void SetHUD(EmotionStats unit)
    {
        hpSlider.maxValue = unit.maxHealth;
        hpSlider.value = unit.health;
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }

}
