using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// MANAGES SCENE TRANSITIONS
public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    public static string previousSceneName;

    // * DRIVER CODE
    // mainly Start() and Update()
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {

    }

    // * DRIVER CODE


    // *** CORE LOGIC
    // these are functions that coordinate smaller functions below
    // can either use scene name or build index
    public void LoadScene(string sceneName)
    {
        // store current scene as a previous scene 
        previousSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(sceneName);
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(previousSceneName);
    }

    public void ExitApplication()
    {
        Application.Quit();
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

    // EVENTS & LISTENERS

}