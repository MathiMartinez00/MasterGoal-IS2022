using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptChip : MonoBehaviour
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
