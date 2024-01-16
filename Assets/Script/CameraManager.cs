using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public Tilemap[] maps;
    public Transform[] mapTrans;
    public int countMap=0;
    public int[] index;
    public GameObject cam;
    

    private void Awake()
    {
        instance = this;
        this.transform.position = new Vector3(0,0,0);
        maps = GetComponentsInChildren<Tilemap>();
        index = new int[maps.Length];
        for(int i=0; i<maps.Length; i++)
        {
            if (maps[i].CompareTag("ground"))
            {
                index[countMap] = i;
                countMap++;
            }
        }

        mapTrans = new Transform[countMap];
        for(int i=0;i<countMap;i++)
        {
            mapTrans[i] = maps[index[i]].transform;

        }

        cam.transform.position = mapTrans[0].position;


    }


}
