using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grids : MonoBehaviour
{
    public Vector2 worldSize;
    public float nodeSize;
    [SerializeField]Node[,] myNode;
    int nodeCountX;
    int nodeCountY;
    [SerializeField] LayerMask obstacle;
    public List<Node> path;
    public bool trigger = false;
    public int x;
    public int y;
    public static Grids instance;


    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        nodeCountX = Mathf.CeilToInt(worldSize.x / nodeSize);
        nodeCountY = Mathf.CeilToInt(worldSize.y / nodeSize);
        myNode = new Node[nodeCountX, nodeCountY];
        for(int i = 0; i < nodeCountX; i++)
        {
            for(int j = 0; j < nodeCountY; j++)
            {
                Vector3 pos = new Vector3(i * nodeSize + 0.08f, j * nodeSize - 3.12f);
                Collider2D hit = Physics2D.OverlapBox(pos, new Vector2(nodeSize/2, nodeSize/2), 0, obstacle);
                bool noHit = false;
                if (hit == null)  noHit = true;
                myNode[i, j] = new Node(noHit, pos,i,j);
            }
        }      
    }

    void Update()
    {
        if(trigger)
        {
            trigger = false;
        }

    }
    public List<Node> SearchNeightborNode(Node node)
    {
        
        List<Node> nodeList = new List<Node>();
        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0) continue;

                int newX = node.myX + i;
                int newY = node.myY + j;

                if (newX >= 0 && newY >= 0 && newX < nodeCountX && newY < nodeCountY)
                {
                    nodeList.Add(myNode[newX,newY]);
                }
            }
        }
        return nodeList;
    }

    public Node GetNodeFromVector(Vector3 vector)
    {
        int posX = Mathf.RoundToInt(Mathf.Abs(vector.x - transform.position.x) / nodeSize);
        int posY = Mathf.RoundToInt(Mathf.Abs(vector.y - transform.position.y) / nodeSize);
        return myNode[posX,posY];
    }

    
 private void OnDrawGizmos()
      {
          Gizmos.DrawWireCube(transform.position, new Vector3(worldSize.x, worldSize.y, 1));
          if (myNode != null)
          {
              foreach(Node no in myNode)
              {
                  Gizmos.color = (no.canWalk) ? Color.white : Color.red;
                if (path != null)
                {
                    if (path.Contains(no))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                  Gizmos.DrawCube(no.myPos, Vector3.one * (nodeSize/2));
              }
          }
      }
    
   
}
