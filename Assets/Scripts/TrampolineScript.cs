using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineScript : MonoBehaviour
{

    private GameObject bubble;
    private float bubbleMinY;
    private float trampolineMinX;
    private float trampolineMaxX;
    private float trampolineMinY;
    private float bubbleCenterX;
    private float bubbleCenterY;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(bubble == null)
        {
            bubble = other.gameObject;
        }
        Bounds circleBounds = bubble.transform.GetComponent<CircleCollider2D>().bounds;
        Bounds boxBounds = transform.GetComponent<BoxCollider2D>().bounds;
        bubbleMinY = circleBounds.min.y;
        trampolineMinX = boxBounds.min.x;
        trampolineMaxX = boxBounds.max.x;
        trampolineMinY = boxBounds.min.y;
        bubbleCenterX = circleBounds.center.x;
        bubbleCenterY = circleBounds.center.y;

        if(bubbleMinY >= trampolineMinY && bubbleCenterX + 0.03f > trampolineMinX && bubbleCenterX - 0.03f < trampolineMaxX && transform.GetComponent<Animator>().GetBool("TrampolineBool") == false)
        {
            transform.GetComponent<Animator>().SetBool("TrampolineBool", true);
            bubble.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 60f);
            transform.GetComponent<BoxCollider2D>().isTrigger = true;
        }

    }

    public void AnimationEnd()
    {
        transform.GetComponent<Animator>().SetBool("TrampolineBool", false);
        transform.GetComponent<BoxCollider2D>().isTrigger = false;
    }
}
