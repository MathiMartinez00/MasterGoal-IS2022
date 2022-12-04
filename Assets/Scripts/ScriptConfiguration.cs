using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptConfiguration : MonoBehaviour
{
    public Toggle toggleHelp;
    public int help = 1; // 1 = help, 0 = no help
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(PlayerPrefs.GetInt("ayuda", 1).ToString());
        if (PlayerPrefs.GetInt("ayuda", 1) == 1)
        {
            toggleHelp.isOn = true;
        }
        else
        {
            toggleHelp.isOn = false;
        }
        toggleHelp.onValueChanged.AddListener(delegate { seeToggle(); });
    }

    public void seeToggle()
    {
        help = toggleHelp.isOn ? 1 : 0;
        PlayerPrefs.SetInt("ayuda", help);
        PlayerPrefs.Save();
    }
}
