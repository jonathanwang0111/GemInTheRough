using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fader : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement playerMovement;
    private List<EnemyAI> enemies;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerMovement>();
    }

    void Start()
    {
        GameObject[] enemyGameObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = new List<EnemyAI>();
        foreach (GameObject g in enemyGameObjects)
        {
            enemies.Add(g.GetComponent<EnemyAI>());
        }
    }

    public void Fade()
    {
        anim.SetBool("Fade", true);
    }

    public void OnFadeOutComplete()
    {
        anim.SetBool("Fade", false);
        playerMovement.SetImmobileState(true);
        foreach (EnemyAI e in enemies)
        {
            e.Reset();
        }
        playerMovement.Reset();
    }

    public void OnFadeInComplete()
    {
        playerMovement.SetImmobileState(false);
    }
}