using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs;    //프리펩 담아두는 배열
    public List<GameObject>[] mobs; 
    //PoolManager의 자식으로 들어가는 게임오브젝트 리스트의 배열. 리스트를 굳이 또 배열로 만든 이유는
    //프리펩스에 여러개의(현재 2개) 프리펩이 들어있기 때문.
    
    public List<Monster> eachmobs;
    //위치정보만을 확인하기위한 리스트

    void Awake()
    {
        mobs = new List<GameObject>[prefabs.Length];
        //프리펩스가 두개이니 배열도 두개로 초기화
        for(int i=0;i<mobs.Length;i++)
        {
            mobs[i] = new List<GameObject>(); //초기화
        }
    }

    void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Keypad2))
        {
            GameObject enemy = Spawn(0);
            
            //enemy.transform.Translate(Collision_Handler.instance.player.rigid.position.x, -0.46f, 1);
        }

        if(Input.GetKeyDown(KeyCode.Keypad3))
        {
            Spawn(1);
        }

        
    }

    public GameObject Spawn(int index)
    {
        GameObject select = null;
        Monster forInit; //Monster 스크립트 내의 Init을 실행하기위함
        foreach (GameObject item in mobs[index]) 
        {
            if(!item.activeSelf) //몹이 비활성화된 상태라면 재활용하여 다시 활성화
            {
                select = item;
                select.SetActive(true);    //활성화시킨 후
                forInit = select.GetComponent<Monster>();//Init 실행(hp, move, position등을 설정)
                forInit.Init();
                eachmobs.Add(forInit);    //포지션 배열에 추가

                
            }
        }

        if(!select)    //비활성화된 것이 없다면, 새로 추가
        {
            select = Instantiate(prefabs[index], transform); //프리펩을 현재 트랜스폼(풀매니저)에 인스턴스화하여 추가
            mobs[index].Add(select);                         //해당 몹 인덱스의 배열에 추가
            select.SetActive(true);                          //활성화하고
            eachmobs.Add(select.GetComponent<Monster>());    //마찬가지로 포지션 추가
            //재활용과는 다르게 Monster스크립트 내의 onenable함수(인스턴스되면 1회 실행)에 Init이 있기에 따로 해주지 않음.
            
            
        }
        return select; //리턴은 현재 스크립트에서는 굳이 필요없음.
    }
    

}
