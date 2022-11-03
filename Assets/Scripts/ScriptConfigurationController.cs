using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScriptConfigurationController : MonoBehaviour
{
    public Button buttonMainMenu;
    public Button buttonCredits;

    // Start is called before the first frame update
    void Start()
    {
        buttonMainMenu.onClick.AddListener(GoSceneMainMenu);
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

    public void GoSceneCredits()
    {
        SceneManager.LoadScene("SceneCredits");
    }
}
