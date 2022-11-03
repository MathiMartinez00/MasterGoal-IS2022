using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptMainMenuController : MonoBehaviour
{
    //button type variables for each button on main menu scene
    public Button buttonConfiguration; 
    public Button buttonCredits;
    public Button buttonPlay;
    public Button buttonTutorial;

    // Start is called before the first frame update
    void Start()
    {
        //listener(action) on each button clicked
        buttonConfiguration.onClick.AddListener(GoSceneConfiguration); 
        buttonCredits.onClick.AddListener(GoSceneCredits);
        buttonPlay.onClick.AddListener(GoSceneGameConfiguration);
        buttonTutorial.onClick.AddListener(GoSceneTutorial); // hay un error de que no encuentra un asset nosque xd
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* 
     *  Loads the scene when the respective button is clicked
     */ 

    public void GoSceneConfiguration()
    {
        //Debug.Log("Button configuration clicked");
        SceneManager.LoadScene("SceneConfiguration"); //load the configuration scene
    }

    public void GoSceneCredits()
    {
        SceneManager.LoadScene("SceneCredits");
    }

    public void GoSceneGameConfiguration()
    {
        SceneManager.LoadScene("SceneGameConfiguration"); 
    }

    public void GoSceneTutorial()
    {
        SceneManager.LoadScene("SceneTutorial");
    }
}
