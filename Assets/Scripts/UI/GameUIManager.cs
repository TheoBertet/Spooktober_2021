using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public LifeScript lifePart;
    public GameObject gameMenu;
    public DeadScreenManager deadScreenManager;

    private bool isGamePaused;

    public void Awake()
    {
        deadScreenManager.gameObject.SetActive(true);
        gameMenu.SetActive(false);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                isGamePaused = true;
                GameManager.Instance.PauseGame();
                gameMenu.SetActive(true);
            }
        }
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        GameManager.Instance.ResumeGame();
        gameMenu.SetActive(false);
    }

    public void SetSouls(int souls)
    {
        lifePart.SetSouls(souls);
    }

    public void AddSoul(int souls)
    {
        lifePart.AddSoul(souls);
    }

    public void ConsumeSoul()
    {
        lifePart.ConsumeSoul();
    }

    public void Die()
    {
        deadScreenManager.Activate();
    }

    public void Damage(int dmg)
    {
        lifePart.Damage(dmg);
    }

    public void Heal(int heal)
    {
        lifePart.Heal(heal);
    }

    public void SetMaxHP(int maxHP)
    {

    }
}
