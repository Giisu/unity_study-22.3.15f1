using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;


public class Layers : MonoBehaviour
{
    public BoxCollider2D[] coll;
    public Transform[] trans;
    public int coll_count;
    public Vector2[] layerPosMin;
    public Vector2[] layerPosMax;



    private void Awake()
    {
        
        coll = GetComponentsInChildren<BoxCollider2D>();
        trans = GetComponentsInChildren<Transform>();
        coll_count = coll.Length;
        layerPosMin = new Vector2[coll_count];
        layerPosMax = new Vector2[coll_count];
        for (int i=0; i<coll.Length; i++)
        {
            Vector2 pos = trans[i+1].position;
            float half_width = coll[i].size.x / 2;
            float half_height = coll[i].size.y / 2;
            layerPosMin[i].x += pos.x - half_width;
            layerPosMin[i].y += pos.y - half_height;
            layerPosMax[i].x += pos.x + half_width;
            layerPosMax[i].y += pos.y + half_height;
        }
    }


}




