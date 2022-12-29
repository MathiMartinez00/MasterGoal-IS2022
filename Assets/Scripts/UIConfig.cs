using UnityEngine;
using UnityEngine.UI;

public class UIConfig : MonoBehaviour
{

    public GameObject gameController;
    public Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        toggle = this.GetComponentInChildren<Toggle>();
        toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(toggle);
        });
    }

    // Update is called once per frame
    void ToggleValueChanged(Toggle change)
    {
        gameController.GetComponent<ScriptController>().isHighlightModeOn = change.isOn;
    }

    public void CloseConfigMenu()
    {
        this.gameObject.SetActive(false);
<<<<<<< HEAD
        gameController.GetComponent<ScriptController>().BoardBoxCollider.enabled = true;
=======
        gameController.GetComponent<ScriptController>().boardBoxCollider.enabled = true;
>>>>>>> a79abc05739dc0b8a020d117a71b2274c279f6ff
    }

    public void OpenConfigMenu()
    {
        this.gameObject.SetActive(true);
<<<<<<< HEAD
        gameController.GetComponent<ScriptController>().BoardBoxCollider.enabled = false;
=======
        gameController.GetComponent<ScriptController>().boardBoxCollider.enabled = false;
>>>>>>> a79abc05739dc0b8a020d117a71b2274c279f6ff
        toggle.isOn = gameController.GetComponent<ScriptController>().isHighlightModeOn;
    }
}
