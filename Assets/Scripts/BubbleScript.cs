using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    [SerializeField]
    private Sprite collisionSprite;
    [SerializeField]
    private Sprite defaultSprite;
    private float collisionTimer = 0f;
    private bool collisionBool = false;
    private Vector3 currentPosition;
    private Vector3 previousPosition = Vector3.zero;
    private GameObject manager;
    private Vector2 staticConstantForce = new Vector2(0, 1.25f);

    public GameObject glass;
    public List<Vector3> guidelinePoints = new List<Vector3>();
    public Vector3 startingPosition;
    public bool isInGlass = true;

    void Start()
    {
        startingPosition = transform.position;
        manager = GameObject.Find("Manager");
    }

    void Update()
    {
        CollisionTimer();
        UpdateTrail();
    }

    private void UpdateTrail()
    {
        if(manager.GetComponent<ManagerScript>().hasBegun)
        {
            currentPosition = transform.position;
            if(currentPosition != previousPosition)
            {
                guidelinePoints.Add(currentPosition);
            }
            previousPosition = currentPosition;
        }
    }

    // On collision, set a srpite.
    private void OnTriggerEnter2D(Collider2D other) {
        if(!other.gameObject.name.StartsWith("Missile"))
        {
            transform.GetComponent<SpriteRenderer>().sprite = collisionSprite;
            collisionBool = true;
        } else
        {
            if(isInGlass)
            {
                isInGlass = false;
                transform.GetComponent<ConstantForce2D>().force = staticConstantForce;
                transform.GetComponent<Rigidbody2D>().gravityScale = 1;
                glass.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    // Making sure that the above method only changes the spirte for a limited time.
    private void CollisionTimer()
    {
        if(collisionBool)
        {
            collisionTimer += Time.deltaTime;
            if(collisionTimer >= 0.5f)
            {
                collisionTimer = 0f;
                collisionBool = false;
                transform.GetComponent<SpriteRenderer>().sprite = defaultSprite;
            }
        }
    }

}

// Wow, for the main character, this script is pretty short.