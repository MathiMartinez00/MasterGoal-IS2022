using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScriptMove : MonoBehaviour
{
    private enum Team
    {
        White,
        Red
    }
    [SerializeField] private Team team;
    // Start is called before the first frame update
    void Start()
    {
    }
}
