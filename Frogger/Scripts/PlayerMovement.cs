using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool hopLock = false;
    bool dead = false, ride = false, bonus = false;
    float ride_direction;
    float ride_speed;
    int ride_ticket = 0;
    Animator playerAnimator;
    Vector3 startingPos, desiredDirection;
    FroggerGM fGM;

    private void Start() {
        playerAnimator = GetComponent<Animator>();
        startingPos = transform.position;
        fGM = GameObject.Find("GameManager").GetComponent<FroggerGM>();
    }
    // Update is called once per frame
    void Update() {
        if (fGM.isPaused()) return;
        if (HandleRespawn()) return;

        if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1) desiredDirection = Vector3.up * Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1) desiredDirection = Vector3.right * Input.GetAxisRaw("Horizontal");

        print(Input.GetAxisRaw("Vertical"));
        print(Input.GetAxisRaw("Horizontal"));

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)) {
            Move(desiredDirection);
        } else if (ride) {
            transform.position += (Vector3.right * ride_direction) * ride_speed * Time.deltaTime;
        }

        if (transform.position.x > 13.5 || transform.position.x < -0.05f) PlayerDeath();
    }

    private void FixedUpdate() {
        if (transform.position.y > 6 && !ride) ride_ticket++;
        else ride_ticket = 0;
        if (ride_ticket>2)PlayerDeath();
        
    }

    void Move(Vector3 direction){

        if (hopLock) return;

        if (direction == Vector3.up) transform.rotation = Quaternion.Euler(Vector3.forward * 0);
        if (direction == Vector3.down) transform.rotation = Quaternion.Euler(Vector3.forward * 180);
        if (direction == Vector3.left) transform.rotation = Quaternion.Euler(Vector3.forward * 90);
        if (direction == Vector3.right) transform.rotation = Quaternion.Euler(Vector3.forward * 270);

        if (transform.position.y == 0 && direction == Vector3.down) return;
        GameObject.Find("Global Control").GetComponent<GlobalControlScript>().PlaySound("Hop");
        playerAnimator.SetTrigger("Hop");
        fGM.AddPoints(10);
        transform.position += direction;
        ride = false;
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Danger")) {
            PlayerDeath();
        }
        if (other.CompareTag("Platform")) {
            EnemyBehaviour eb = other.GetComponent<EnemyBehaviour>();
            ride = true;
            ride_direction = eb.direction.x;
            ride_speed = eb.speed;
        }
        if (other.CompareTag("Win")) {
            fGM.ResetTimer();
            EnemyBehaviour eb = other.GetComponent<EnemyBehaviour>();
            if (eb.Land())
                fGM.AddPoints(200);
            if (RemoveChildren())
                fGM.AddPoints(200);
            transform.position = startingPos;
        }
        if (other.CompareTag("Bonus")&&!bonus) {
            EnemyBehaviour eb = other.GetComponent<EnemyBehaviour>();
            eb.Pickup();
        }
    }
    
    public void PlayerDeath() {
        if (dead) return;
        bonus = false;
        RemoveChildren();
        if (transform.position.y > 6) playerAnimator.SetTrigger("Sink");
        else playerAnimator.SetTrigger("Squish");
        GameObject.Find("Global Control").GetComponent<GlobalControlScript>().PlaySound("Death");
        transform.rotation = Quaternion.Euler(Vector3.forward * 0);
        ride_ticket = 0;
        ride = false;
        dead = true;
        fGM.StopTimer();
        fGM.lives--;
    }

    bool HandleRespawn() {
        if (dead && playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Respawn")) {
            transform.position = startingPos;
            playerAnimator.SetTrigger("Spawn");
            ride = false;
            dead = false;
            fGM.StartTimer();
            return true;
        }
        return dead;
    }

    bool RemoveChildren() {
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        if (children.Length == 1) return false;
        foreach(Transform t in children) {
            if (t == this.transform) continue;
            Destroy(t.gameObject);
        }
        return true;
    }
}
