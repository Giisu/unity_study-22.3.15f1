using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class no_passing : MonoBehaviour
{
    private BoxCollider2D wall;

    // Start is called before the first frame update
    void Start()
    {
        wall = GetComponent<BoxCollider2D>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
            this.transform.position = this.transform.position;
    }
}
