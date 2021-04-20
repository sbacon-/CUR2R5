using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour
{
    public Vector3 
        desiredDirection = Vector3.right, 
        activeDirection = Vector3.right;

    GameManager gm;

    [SerializeField] private GameObject tailSegment;

    public float speed = 150   ;
    private void Start() {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        InvokeRepeating("UpdateMovement", 3f, 10/speed);
        transform.position = Vector3.zero;
    
    }
    // Update is called once per frame
    void Update() {
        //Change Direction 
        if (gm.gameStart) {
            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1 && activeDirection.z == 0) {
                desiredDirection = new Vector3(0, 0, Input.GetAxisRaw("Vertical"));
            }
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1 && activeDirection.x == 0) {
                desiredDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
            }
        }
    }

    void UpdateMovement() {
        if (!gm.gameStart) gm.gameStart = true;
        Vector3 pos = transform.position;
        transform.position += desiredDirection;
        activeDirection = desiredDirection;
        tailSegment.GetComponent<SegmentBehaviour>().UpdateMovement(pos);
    }

    private void OnTriggerEnter(Collider other) {
        print(other.tag);
        if (other.CompareTag("Pickup")) {
            gm.AddSegment();
            speed += 10;
            CancelInvoke("UpdateMovement");
            InvokeRepeating("UpdateMovement", 0, 10 / speed);
            GameObject.Destroy(other.gameObject);
            GlobalControlScript gcs = GameObject.Find("Global Control").GetComponent<GlobalControlScript>();
            gcs.PlaySound("SnakePickup");
        }
        if (other.CompareTag("Player") || other.CompareTag("Wall")){
            gm.GameOver();
            CancelInvoke("UpdateMovement");
            GlobalControlScript gcs = GameObject.Find("Global Control").GetComponent<GlobalControlScript>();
            gcs.PlaySound("SnakeDeath");
        }
    }
}
