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
        gameController.GetComponent<ScriptController>().BoardBoxCollider.enabled = true;
    }

    public void OpenConfigMenu()
    {
        this.gameObject.SetActive(true);
        gameController.GetComponent<ScriptController>().BoardBoxCollider.enabled = false;
        toggle.isOn = gameController.GetComponent<ScriptController>().isHighlightModeOn;
    }
}
