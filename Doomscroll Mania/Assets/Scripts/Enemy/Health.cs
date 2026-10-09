using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;

    // * DRIVER CODE
    // mainly Start() and Update()

    // * DRIVER CODE


    // *** CORE LOGIC
    // these are functions that coordinate smaller functions below
    public void ChangeHealth(float amount)
    {
        currentHealth += amount;

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);

            //OnEnemyDeath?.Invoke(); // for transitioning back to node progression scene ?
            //GameSceneManager.Instance.LoadScene("NodeProgressionScene"); // or this, for now :D 
        }
    }
    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions

    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)

    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here

    //public static event Action OnEnemyDeath;

    // EVENTS & LISTENERS
}
