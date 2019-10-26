using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Vector2 topRightBound;
    [SerializeField] private Vector2 bottomLeftBound;
    [SerializeField] private Camera mainCamera;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    private void Awake()
    {
        float vertExtent = mainCamera.orthographicSize;    
        float horzExtent = vertExtent * Screen.width / Screen.height;
 
         // Calculations assume map is position at the origin
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            // Getting direction based on input

            Vector2 direction = Vector2.zero;
            if(Input.GetKey(KeyCode.W))
            {
                direction = new Vector2(0, 1);
            } else if(Input.GetKey(KeyCode.A))
            {
                direction = new Vector2(-1, 0);
            } else if(Input.GetKey(KeyCode.S))
            {
                direction = new Vector2(0, -1);
            } else
            {
                direction = new Vector2(1, 0);
            }

            Vector3 newDirection = new Vector3(direction.x * 0.04f, direction.y * 0.04f, 0);
            Vector3 pos = mainCamera.transform.position;
            pos += newDirection;

            float screenAspect = (float)Screen.width / (float)Screen.height;
            float cameraHeight = mainCamera.orthographicSize * 2;
            Bounds bounds = new Bounds(
                pos,
                new Vector3(cameraHeight * screenAspect, cameraHeight, 0));

            // Moving the camera

            if(bounds.max.x < topRightBound.x && bounds.max.y < topRightBound.y && bounds.min.x > bottomLeftBound.x && bounds.min.y > bottomLeftBound.y)
            {
                mainCamera.transform.position += newDirection;
            }
        }
    }
}
