using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptChangeScene : MonoBehaviour
{
    public Button button;
    public void GoScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
