﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropsProperty : MonoBehaviour
{
    public GameObject mainThread;
    public int id;
    public float r;
    private Rigidbody2D rb2D;
    private bool willShrink = false;
    private bool willSwell = false;
    private float collidedR = 0.0f;
    private float originR = 0.0f;
    float deltaX;
    float deltaY;
    private Vector2 collidePosition;
    // Input
#if UNITY_EDITOR
    private bool rightForce;
    private bool leftForce;
    private bool upForce;
    private bool downForce;
#else
    private Quaternion inputForce;
#endif

    void Start()
    {
        rb2D = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // detect input
        #if UNITY_EDITOR
            rightForce = Input.GetKey(KeyCode.D);
            leftForce = Input.GetKey(KeyCode.A);
            upForce = Input.GetKey(KeyCode.W);
            downForce = Input.GetKey(KeyCode.S);
            if (rightForce || leftForce || upForce || downForce && !MainThread.GameStarted) {
                MainThread.GameStarted = true;
            }
            if (rightForce) rb2D.AddForce(new Vector3(r/2, 0.0f, 0.0f));
            if (leftForce) rb2D.AddForce(new Vector3(-r/2, 0.0f, 0.0f));
            if (upForce) rb2D.AddForce(new Vector3(0.0f, r/2, 0.0f));
            if (downForce) rb2D.AddForce(new Vector3(0.0f, -r/2, 0.0f));
        #else
            inputForce = Input.gyro.attitude;
            if (inputForce && !MainThread.GameStarted) {
                MainThread.GameStarted = true;
            }
            if (inputForce) {
                rb2D.AddForce(new Vector3(inputForce.x*r/2, 0.0f, 0.0f));
                rb2D.AddForce(new Vector3(0.0f, inputForce.y*r/2, 0.0f));
            }
        #endif

        if (willShrink){
            this.r -= Mathf.Sqrt(Mathf.Pow(deltaX, 2)+Mathf.Pow(deltaY, 2))/this.r;
            this.gameObject.transform.localScale = new Vector2(this.r, this.r);
            this.transform.Translate(deltaX/this.r, deltaY/this.r, 0.0f);
            if (this.r<=0.0f) Destroy(this.gameObject);
        }
        if (willSwell){
            this.r += Mathf.Sqrt(Mathf.Pow(deltaX, 2)+Mathf.Pow(deltaY, 2))/collidedR;
            this.gameObject.transform.localScale = new Vector2(this.r, this.r);
            if (this.r>=(collidedR/2+originR)) willSwell = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!MainThread.GameStarted) return;
        if (other.gameObject.GetComponent<DropsProperty>()){
            // collide with other drops
            if(this.r >= other.gameObject.GetComponent<DropsProperty>().r){
                willSwell = true;
                originR = this.r;
                collidedR = other.gameObject.GetComponent<DropsProperty>().r;
                // if (MainThread.score==0) MainThread.score += System.Convert.ToInt32(originR/2.0);
                MainThread.score += System.Convert.ToInt32(collidedR/4.0);
            }else{
                willShrink = true;
            }
            float ratio = this.r/(this.r+other.gameObject.GetComponent<DropsProperty>().r);
            deltaX = (other.gameObject.transform.position.x - this.transform.position.x) * ratio;
            deltaY = (other.gameObject.transform.position.y - this.transform.position.y) * ratio;
        }else{
            // collide with walls
            // Destroy(this.gameObject);
        }
    }
}
