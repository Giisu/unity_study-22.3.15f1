using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space) && Collision_Handler.instance.spaceTrigger)
            Collision_Handler.instance.gravity = "jumping";
 
        Collision_Handler.instance.MovingVec();
        Collision_Handler.instance.GravityVec();
        Collision_Handler.instance.GetPlayerColl();
        Collision_Handler.instance.CollisionCheck();
        Repos();

    }

    public void Repos()
    {
        rigid.MovePosition(rigid.position + Collision_Handler.instance.horiVec + Collision_Handler.instance.vertVec);
    }

}
