using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transfer : MonoBehaviour
{
    public BoxCollider2D[] coll;
    public Transform[] objects;
    public int coll_count;
    public BoxCollider2D colls;

    public MapData[] data;
    


    void Awake()
    {
        coll = GetComponentsInChildren<BoxCollider2D>();
        coll_count = coll.Length;
        objects = new Transform[coll.Length];
        data = new MapData[coll.Length];
        for(int i=0;i<data.Length;i++)
        {
            data[i] = new MapData();
        }
        
        for (int i=0; i<coll.Length; i++)
        {
            objects[i] = transform.GetChild(i);
            Vector2 pos = objects[i].position;
            float half_width = coll[i].size.x / 2;
            float half_height = coll[i].size.y / 2;
            data[i].minPos = Vector2.zero;
            data[i].maxPos = Vector2.zero;
            data[i].minPos.x = pos.x - half_width;
            data[i].minPos.y = pos.y - half_height;
            data[i].maxPos.x = pos.x + half_width;
            data[i].maxPos.y = pos.y + half_height;
            data[i].mapNum = Convert.ToInt32(objects[i].name.Substring(3,1));
            data[i].whichSide = objects[i].name.Substring(5);
            data[i].currentPos = pos;

        }
        
        
    }

    [System.Serializable]
    public class MapData
    {
        public int mapNum;
        public string whichSide;
        public Vector2 minPos;
        public Vector2 maxPos;

        public Vector2 currentPos;
    }



}
