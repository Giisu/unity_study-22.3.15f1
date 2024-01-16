using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transfer transfer;

    void Awake()
    {
        transfer = GetComponent<Transfer>();
    }
}
