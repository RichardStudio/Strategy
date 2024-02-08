using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Information : MonoBehaviour
{

    [HideInInspector] public bool endGame = false;
    public GameObject Warrior;
    public GameObject Spear;
    public GameObject Knight;

    public GameObject[] GetUnitsArr()
    {
        return new GameObject[] { Warrior, Spear, Knight };
    }
}
