using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public Vector3 direction;
    public float speed;

    bool rose=false;

    private void Start() {
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        if (direction == Vector3.left && transform.position.x < -2) {
            transform.position = new Vector3(transform.position.x + 16, transform.position.y, transform.position.z);
        }
        if (direction == Vector3.right && transform.position.x > 14) {
            transform.position = new Vector3(transform.position.x - 16, transform.position.y, transform.position.z);
        }


        //TURLE_SINK
        if (gameObject.name.StartsWith("Turtle_Sink")) {
            if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Sank") && !rose) {
                GetComponent<SpriteRenderer>().color = new Color(1,1,1,0);
                tag = "Danger";
                Invoke("TurtleSinkRise", 1f);
                rose = true;
            };
        }

        

    }

    public bool Land() {
        CancelInvoke("ResetHome");
        Animator anim = GetComponent<Animator>();
        bool fly = anim.GetBool("Fly");
        anim.SetTrigger("Land");
        anim.SetBool("Fly", false);
        GameObject.Find("Global Control").GetComponent<GlobalControlScript>().PlaySound("Land");
        GameObject.Find("EnemyManager").GetComponent<EnemyManager>().lands++;
        this.tag = "Danger";
        
        return fly;        
    }
    void TurtleSinkRise() {
        GetComponent<Animator>().SetTrigger("Rise");
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        tag = "Platform";
        rose = false;
    }

    internal void HomeFly() {
        this.GetComponent<Animator>().SetBool("Fly",true);
        Invoke("ResetHome", 8);
    }

    internal void HomeAlligator() {
        this.GetComponent<Animator>().SetTrigger("Alligator");
        this.tag = "Danger";
        Invoke("ResetHome", 8);
    }

    internal void ResetHome() {
        this.tag = "Win";
        Animator anim = this.GetComponent<Animator>();
        anim.SetBool("Fly", false);
        anim.SetTrigger("Blank");
    }
    
    internal void Pickup() {
        CancelInvoke("SelfDestruct");
        this.gameObject.transform.parent = GameObject.Find("Player").transform;
        this.gameObject.transform.localPosition = Vector3.forward * -0.05f;
        Destroy(this);
    }

    internal void SelfDestruct(int offset) {
        Invoke("SelfDestruct",offset);
    }
    internal void SelfDestruct() {
        Destroy(this.gameObject);
    }
}
