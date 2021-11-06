using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LifeScript : MonoBehaviour
{
    public int maxHP;
    public Image jaugeUI;
    public TextMeshProUGUI soulsScore;

    private int HP;
    private int souls;

    // Start is called before the first frame update
    void Start()
    {
        HP = maxHP;
    }

    public void SetSouls(int souls)
    {
        this.souls = souls;
        Refresh();
    }

    public void AddSoul(int souls)
    {
        this.souls += souls;
        Refresh();
    }

    public void ConsumeSoul()
    {
        souls--;
        Refresh();
    }

    public void Damage(int dmg)
    {
        HP -= dmg;
        Refresh();
    }

    public void Heal(int heal)
    {
        HP += heal;
        Refresh();
    }

    public void SetMaxHP(int max)
    {
        maxHP = max;
        HP = maxHP;
        Refresh();
    }

    public void Refresh()
    {
        jaugeUI.fillAmount = (float)HP / (float)maxHP;
        soulsScore.SetText(souls.ToString());
    }
}
