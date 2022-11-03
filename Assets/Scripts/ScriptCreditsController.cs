using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptCreditsController : MonoBehaviour
{
    //button type variables for each button on main menu scene
    public Button buttonConfiguration, buttonMainMenu;

    // Start is called before the first frame update
    void Start()
    {
        //listener(action) on each button clicked
        buttonConfiguration.onClick.AddListener(GoSceneConfiguration);
        buttonMainMenu.onClick.AddListener(GoSceneMainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Load the respective scene

    public void GoSceneConfiguration()
    {
        SceneManager.LoadScene("SceneConfiguration");
    }

    public void GoSceneMainMenu()
    {
        SceneManager.LoadScene("SceneMainMenu");
    }

}
//TODO: intentar reutilizar los GoScene de otros .cs (intente pero necesito referenciar la escena donde esta el .cs que tiene la funcion que quiero y parece que no es recomendable xd)