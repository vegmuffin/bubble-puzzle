﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileScript : MonoBehaviour
{
    [SerializeField]
    private Sprite explosionSprite;
    [SerializeField]
    private Sprite defaultSprite;
    private float explosionTimer = 0f;
    private bool explosionBool = false;
    private float moveSpeed = 0.05f;
    private Vector3 startingPosition;
    private bool increaseSizeBool = true;

    public float angle;
    public bool startBool = false;

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        ExplosionTimer();
        Move();
        IncreaseSize();
    }

    // Moving the missile with coordinates and a touch of math.
    private void Move()
    {
        if(startBool)
        {
            float x = transform.position.x;
            float y = transform.position.y;
            x += moveSpeed * Mathf.Cos(angle);
            y += moveSpeed * Mathf.Sin(angle);
            transform.position = new Vector3(x, y, transform.position.z);

            if(Vector3.Distance(transform.position, startingPosition) >= 100f)
            {
                Destroy(transform.gameObject);
            }
        }
        
    }

    // Quickly increasing the size of the missile when it has been instantiated to make it look like it hasn't appeared out of nowhere.
    private void IncreaseSize()
    {
        if(increaseSizeBool)
        {
            transform.localScale = new Vector3(transform.localScale.x + 0.03f, transform.localScale.y + 0.03f, transform.localScale.z + 0.03f);
            if(transform.localScale.x >= 1f)
            {
                transform.localScale = new Vector3(1, 1, 1);
                increaseSizeBool = false;
            }
        }
    }

    // On collision, destroy the missile and add force to the bubble.
    private void OnTriggerEnter2D(Collider2D other) {
        float x1 = other.transform.position.x;
        float y1 = other.transform.position.y;
        float x2 = transform.position.x;
        float y2 = transform.position.y;
        float angle = Mathf.Atan2(y1 - y2, x1 - x2)*180 / Mathf.PI;
        Vector3 dir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;

        other.transform.GetComponent<Rigidbody2D>().AddForce(dir * 40f); // Modifiable force
        other.transform.GetComponent<SpriteRenderer>().sprite = explosionSprite;

        Destroy(transform.gameObject);
    }

    // Timer so we could set bubble sprite later.
    private void ExplosionTimer()
    {
        if(explosionBool)
        {
            explosionTimer += Time.deltaTime;
            if(explosionTimer >= 2f)
            {
                explosionTimer = 0f;
                explosionBool = false;
                GameObject.Find("Bubble").transform.GetComponent<SpriteRenderer>().sprite = defaultSprite;
            }
        }
    }
}
