using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


public class Timer : MonoBehaviour
{
    Text timer;
    int time = 0;
    float timeflow = 0;
    int min;
    int sec;

    void Awake()
    {
        timer = GetComponent<Text>();
    }

    void Update()
    {
        timeflow += Time.deltaTime;
        min = Mathf.FloorToInt(timeflow / 60);
        sec = Mathf.FloorToInt(timeflow % 60);
        timer.text = string.Format("{0:D2}:{1:D2}", min, sec);
        

    }
}
