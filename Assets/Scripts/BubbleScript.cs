using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    public static BubbleScript instance;

    [SerializeField] private Sprite collisionSprite;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private GameObject cameraParent;
    private float collisionTimer = 0f;
    private bool collisionBool = false;
    private Vector3 currentPosition;
    private Vector3 previousPosition = Vector3.zero;
    private Vector2 staticConstantForce = new Vector2(0, 1.25f);
    [SerializeField] private ParticleSystem collisionPS;

    public GameObject glass;
    public List<Vector3> guidelinePoints = new List<Vector3>();
    public Vector3 startingPosition;
    public bool isInGlass = true;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        startingPosition = transform.position;
    }

    private void FixedUpdate()
    {
        CollisionTimer();
        UpdateTrail();
    }

    private void UpdateTrail()
    {
        if(ManagerScript.instance.GetComponent<ManagerScript>().hasBegun)
        {
            previousPosition = currentPosition;
            currentPosition = transform.position;
            if(currentPosition != previousPosition)
            {
                guidelinePoints.Add(currentPosition);
            }
        }
    }

    // On collision, set a srpite.
    private void OnTriggerEnter2D(Collider2D other)
    {
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
            } else if(other.gameObject.name.StartsWith("Missile"))
            {
                ManagerScript.instance.GetComponent<GuidelineImageScript>().explosionPoints.Add(transform.position);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(!other.gameObject.name.StartsWith("Missile") && !other.gameObject.name.StartsWith("Trampoline") && !isInGlass)
        {
            if(!ManagerScript.instance.GetComponent<ManagerScript>().isInTheBox && other.gameObject.name != "SoftTilemap")
            {
                if(Vector3.Distance(previousPosition, currentPosition) >= 0.05f)
                {
                    if(CameraShake.instance.timer == 0.5f)
                        CameraShake.instance.StartCoroutine(CameraShake.instance.ShakeCamera());
                    else
                        CameraShake.instance.timer = 0.5f;
                }
                    
                Bounce(transform.position, other.contacts[0].point, transform.GetComponent<Rigidbody2D>());

                Vector3 cpsPosition = other.contacts[0].point;
                GameObject cps = Instantiate(collisionPS.gameObject, cpsPosition, Quaternion.identity);
                Destroy(cps, 0.5f);
            }

            transform.GetComponent<SpriteRenderer>().sprite = collisionSprite;
            collisionBool = true;
        }
    }

    // Making sure that the above method only changes the spirte for a limited time.
    private void CollisionTimer()
    {
        if(collisionBool)
        {
            collisionTimer += Time.fixedDeltaTime;
            if(collisionTimer >= 0.5f)
            {
                collisionTimer = 0f;
                collisionBool = false;
                transform.GetComponent<SpriteRenderer>().sprite = defaultSprite;
            }
        }
    }

    private void Bounce(Vector2 bubblePos, Vector2 collisionPos, Rigidbody2D rb)
    {
        // Getting the angle
        float x1 = bubblePos.x;
        float y1 = bubblePos.y;
        float x2 = collisionPos.x;
        float y2 = collisionPos.y;
        float angle = Mathf.Atan2(y1 - y2, x1 - x2) * 180 / Mathf.PI;

        Vector3 dir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;

        // Getting velocity
        float vel = Vector2.Distance(currentPosition, previousPosition);

        // Adding amplified force
        rb.AddForce(dir*300f*vel);
    }

    public void ResetSprite()
    {
        transform.GetComponent<SpriteRenderer>().sprite = defaultSprite;
    }

}
