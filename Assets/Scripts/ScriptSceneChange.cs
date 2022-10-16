using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptSceneChange : MonoBehaviour
{

    public Button m_button;
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        m_button.onClick.AddListener(() => SceneChange(sceneName));
    }

    void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
