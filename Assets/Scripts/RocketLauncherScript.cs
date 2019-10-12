using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class RocketLauncherScript : MonoBehaviour
{

    [SerializeField]
    private InputField inputField;
    [SerializeField]
    private GameObject selectionCircle;
    [SerializeField]
    private GameObject whiteArrow;
    [SerializeField]
    private GameObject missile;
    [SerializeField]
    private Text timerText;
    private GameObject manager;
    private float movement = 0f;
    private float missileAngle = -90f;
    private GameObject whiteArrowInstance;
    private LineRenderer lineRenderer;

    public Vector3 staticTextPosition;
    public float staticMissileTimer;
    public GameObject missileInstance;
    public InputField inputFieldInstance;
    public Text timerTextInstance;
    public int launcherNumber;
    public float missileTimer = 0f;

    // Getting some references.
    void Awake()
    {
        manager = GameObject.Find("Manager");
        lineRenderer = manager.GetComponent<ManagerScript>().lineRenderer;
    }

    void Update()
    {
        UpdateAngle();
        RunMissileTimer();
        KeepTextPosition();
    }
    
    // On mouse down - create a white arrow, set up line renderer, input field, green circle and tell the manager that this launcher is selected.
    private void OnMouseDown() {
        if(launcherNumber != manager.GetComponent<ManagerScript>().whichSelected && !manager.GetComponent<ManagerScript>().hasBegun)
        {
            Thread.Sleep(20);
            DestroySelection();

            manager.GetComponent<ManagerScript>().whichSelected = launcherNumber;
            manager.GetComponent<ManagerScript>().lineRenderer.enabled = true;

            Instantiate(selectionCircle, transform.position, Quaternion.identity);

            Vector3 inputFieldPosition = new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z);
            inputFieldPosition = Camera.main.WorldToScreenPoint(inputFieldPosition);
            GameObject mainCanvas = GameObject.Find("Canvas");
            if(inputFieldInstance == null)
            {
                inputFieldInstance = Instantiate(inputField, inputFieldPosition, Quaternion.identity, mainCanvas.transform);
                inputFieldInstance.GetComponent<IFCustom>().inputFieldLauncherNumber = launcherNumber;
            }
            else
            {
                inputFieldInstance.gameObject.SetActive(true);
            }

            if(whiteArrowInstance == null)
            {
                Vector3 arrowPosition = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
                whiteArrowInstance = Instantiate(whiteArrow, arrowPosition, Quaternion.identity, transform);
            }

            // Input field is only deactivated on end edit, we have to take care of whether it wasnt edited in its session
            // This creates a bit of an issue where you select another launcher instantly, this is a to do
            foreach(GameObject launcher in manager.GetComponent<ManagerScript>().launcherArray)
            {
                if(launcher.GetComponent<RocketLauncherScript>().inputFieldInstance != null && launcher.GetComponent<RocketLauncherScript>().launcherNumber != manager.GetComponent<ManagerScript>().whichSelected)
                {
                    launcher.GetComponent<RocketLauncherScript>().inputFieldInstance.gameObject.SetActive(false);
                }
            }
        }
        
    }

    // Updating the angle for white arrow as well as the line renderer when axis input is detected.
    private void UpdateAngle()
    {
        if(launcherNumber == manager.GetComponent<ManagerScript>().whichSelected)
        {

            movement = Input.GetAxisRaw("Horizontal");
            whiteArrowInstance.transform.RotateAround(transform.position, Vector3.forward, movement * Time.deltaTime * 75f); // Modifiable move speed of the arrow

            float x1 = transform.position.x;
            float y1 = transform.position.y;
            float x2 = whiteArrowInstance.transform.position.x;
            float y2 = whiteArrowInstance.transform.position.y;
            missileAngle = Mathf.Atan2(y2 - y1, x2 - x1)*180 / Mathf.PI;

            Vector3 lrPos1 = whiteArrowInstance.transform.position;
            float x3 = whiteArrowInstance.transform.position.x + 2f * Mathf.Cos(missileAngle*Mathf.Deg2Rad);
            float y3 = whiteArrowInstance.transform.position.y + 2f * Mathf.Sin(missileAngle*Mathf.Deg2Rad);
            Vector3 lrPos2 = new Vector3(x3, y3, 0);
            lineRenderer.SetPosition(0, lrPos1);
            lineRenderer.SetPosition(1, lrPos2);
        }
        
    }

    // Setting up everything that is necessary when entering 'play mode' (preparing the missile, deactivating the input field)
    public void InitiateLauncher()
    {
        staticMissileTimer = missileTimer;
        missileInstance = Instantiate(missile, transform.position, Quaternion.identity);
        missileInstance.GetComponent<MissileScript>().transform.eulerAngles = new Vector3(0, 0, missileAngle - 90f);
        missileInstance.GetComponent<MissileScript>().angle = missileAngle * Mathf.Deg2Rad;

        if(inputFieldInstance.gameObject.activeSelf)
        {
            inputFieldInstance.gameObject.SetActive(false);
        }
    }

    // When I'm not lazy, I could have two seperate canvases, one in world space and one as an overlay to avoid having to stabilize text position.
    private void KeepTextPosition()
    {
        if(manager.GetComponent<ManagerScript>().hasBegun)
        {
            timerTextInstance.transform.position = Camera.main.WorldToScreenPoint(staticTextPosition);
        }
    }

    // Run the timer, after it ends, the missiles are launched.
    private void RunMissileTimer()
    {
        if(manager.GetComponent<ManagerScript>().hasBegun)
        {
            if(missileTimer >= 0)
            {
                missileTimer -= Time.deltaTime;
                timerTextInstance.text = missileTimer.ToString("F2");
            } else
            {
                if(missileInstance != null)
                {
                    whiteArrowInstance.GetComponent<SpriteRenderer>().enabled = false;
                    missileInstance.GetComponent<MissileScript>().startBool = true;
                    timerTextInstance.text = "0";
                }
            }
        }
    }

    // This is was initially more useful as the trash tag included more objects, now it's just to destroy the green circle if I remember correctly.
    private void DestroySelection()
    {
        foreach(GameObject trash in GameObject.FindGameObjectsWithTag("Trash"))
        {
            Destroy(trash);
        }
    }

}
