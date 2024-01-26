using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    Transform[] hpImage = new Transform[3];
    const int count = 3;

    public int index = 3;
    bool isActive;


    void Awake()
    {
        for(int i=0; i<3; i++)
        {
            hpImage[i] = this.gameObject.GetComponentsInChildren<Transform>()[i+1];
            // if(i>0)
            //     hpImage[i].gameObject.SetActive(false);
        }
    }
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Keypad0))
            index--;

        if(Input.GetKeyDown(KeyCode.Keypad1))
            index++;
        
        index = Mathf.Clamp(index,0,3);

        if(index == 0)
            Collision_Handler.instance.Gameover();

        

    }
    void LateUpdate()
    {
        for(int i=0;i<count;i++)
        {
            isActive = i < index? true : false;
            hpImage[i].gameObject.SetActive(isActive);
        }
    }



}
