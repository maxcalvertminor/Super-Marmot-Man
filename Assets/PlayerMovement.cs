using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;

    public float playerHeight;
    public float distance;
    public float groundSpeed;
    public float airSpeed;

    [SerializeField] private float kickSpeed;
    public float addedSpeed;
    public float minimumKickSpeed;
    [SerializeField] private int chainedKicks;

    public bool grounded;
    [SerializeField] private float groundedLockTimer;
    public float groundedLockTimerCooldown;

    public GameObject kickHitBox;
    public GameObject pivot;

    public Camera mainCam;
    public float fineTuningConstant;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(groundedLockTimer > 0) {
            groundedLockTimer -= Time.deltaTime;
        }

        Vector2 camPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        pivot.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(camPos.y - pivot.transform.position.y, camPos.x - pivot.transform.position.x) * Mathf.Rad2Deg + fineTuningConstant);
        //Debug.Log("" + (camPos.y - transform.position.y) + ", " + (camPos.x - transform.position.x));

        if(grounded) {
            GetComponent<Rigidbody2D>().velocity = new Vector2((int)Input.GetAxis("Horizontal") * groundSpeed, 0);
        } else {
            GetComponent<Rigidbody2D>().AddForce(new Vector2((int)Input.GetAxis("Horizontal") * airSpeed, 0));
        }

        if(Input.GetButtonDown("Kick")) {
            if(Physics2D.OverlapBox(kickHitBox.transform.position, kickHitBox.transform.localScale, kickHitBox.transform.parent.rotation.eulerAngles.z, LayerMask.GetMask("Walls"))) {
                body.velocity = new Vector2(0, 0);
                chainedKicks++;
                kickSpeed = minimumKickSpeed + (addedSpeed * chainedKicks);
                grounded = false;
                groundedLockTimer = groundedLockTimerCooldown; 
                GetComponent<Rigidbody2D>().AddForce(new Vector2(camPos.x - transform.position.x, camPos.y - transform.position.y).normalized * kickSpeed, ForceMode2D.Impulse);
            }
        }
        
    }

    void FixedUpdate() {
        if(groundedLockTimer <= 0 && Physics2D.Raycast(transform.position - new Vector3(0, playerHeight/2, 0), new Vector2(0, -1), distance, LayerMask.GetMask("Walls"))) {
            grounded = true;
            chainedKicks = 0;
        } else {
            grounded = false;
        }
    }
}
