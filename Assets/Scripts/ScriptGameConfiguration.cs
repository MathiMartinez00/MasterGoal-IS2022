using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptGameConfiguration : MonoBehaviour
{
    public Button buttonMainMenu, buttonConfiguration, buttonCredits;

    // Start is called before the first frame update
    void Start()
    {
        buttonMainMenu.onClick.AddListener(GoSceneMainMenu);
        buttonConfiguration.onClick.AddListener(GoSceneConfiguration);
        buttonCredits.onClick.AddListener(GoSceneCredits);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoSceneMainMenu()
    {
        SceneManager.LoadScene("SceneMainMenu");
    }
    public void GoSceneConfiguration()
    {
        SceneManager.LoadScene("SceneConfiguration");
    }
    public void GoSceneCredits()
    {
        SceneManager.LoadScene("SceneCredits");
    }
}
