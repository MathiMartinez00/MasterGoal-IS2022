using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScriptBoard : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] GameObject controllerObject;

    public void OnPointerDown(PointerEventData eventData)
    {
        controllerObject.GetComponent<ScriptController>().UpdateBoard(eventData);
    }
}
