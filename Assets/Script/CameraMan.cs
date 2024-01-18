using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMan : MonoBehaviour
{
    Vector3[] positions;
 


    void Awake()
    {
        positions = new Vector3[4];
        positions[0] = new Vector3(1.596f, -1.601f, -1f);
        positions[1] = new Vector3(5.111f, -1.601f, -1f);
        positions[2] = new Vector3(8.600f, -1.601f, -1f);
        positions[3] = new Vector3(12.111f, -1.601f, -1f);
        
    }

    void Update()
    {
        this.transform.position = positions[Collision_Handler.instance.player.currnetMap-1];
    }



}
