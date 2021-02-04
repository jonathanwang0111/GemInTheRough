using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReachGoal : MonoBehaviour
{
    public WinScreen winScreen;
    public PauseMenuInformation pauseMenu;

    void Start()
    {
        winScreen.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Gem")
        {
            winScreen.gameObject.SetActive(true);
            pauseMenu.gameObject.SetActive(false);
        }
    }
}
