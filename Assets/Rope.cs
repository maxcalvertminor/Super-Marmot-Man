using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public List<GameObject> segments, pool;
    public GameObject player, ropeSegment;
    public LineRenderer lineRenderer;
    public int jointNumber;
    public float jointLength;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < jointNumber; i++) {
            pool.Add(Instantiate(ropeSegment));
            pool[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Rigidbody2D GenerateRope(Vector2 grapplePoint) {
        jointLength = Vector2.Distance(player.transform.position, grapplePoint) / jointNumber;
        Vector2 offset = new Vector2(player.transform.position.x - grapplePoint.x, player.transform.position.y - grapplePoint.y) / jointNumber;
        float zRotation = Mathf.Atan2(player.transform.position.y - grapplePoint.y, player.transform.position.x - grapplePoint.x) * Mathf.Rad2Deg;

        for(int i = 0; i < jointNumber; i++) {
            pool[i].SetActive(true);
            pool[i].transform.position = grapplePoint + (offset * i);
            pool[i].transform.rotation = Quaternion.Euler(0, 0, zRotation);
            pool[i].transform.localScale = new(.1f, jointLength, 0);
            if(i == 0) {
                pool[i].GetComponent<HingeJoint2D>().connectedAnchor = grapplePoint;
                continue;
            }
            pool[i].GetComponent<HingeJoint2D>().connectedBody = pool[i-1].GetComponent<Rigidbody2D>();
            pool[i].GetComponent<HingeJoint2D>().connectedAnchor = new(0, -jointLength);
        }

        return pool[pool.Count-1].GetComponent<Rigidbody2D>();
    }
}
