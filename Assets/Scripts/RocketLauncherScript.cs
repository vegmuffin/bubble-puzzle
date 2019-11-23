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
    [SerializeField]
    private AudioClip audioClip;
    [SerializeField]
    private GameObject rightButtonPrefab;
    [SerializeField]
    private GameObject leftButtonPrefab;
    [SerializeField] private AudioClip selection;
    private float movement = 0f;
    private float missileAngle = -90f;
    private LineRenderer lineRenderer;
    private float previousAngle;
    private float soundTimer = 0f;
    private GameObject rightButton;
    private GameObject leftButton;
    private Vector3 textPosition;
    private GameObject wsCanvas;

    public Vector3 staticTextPosition;
    public float staticMissileTimer;
    public GameObject missileInstance;
    public InputField inputFieldInstance;
    public Text timerTextInstance;
    public int launcherNumber;
    public float missileTimer = 0f;
    public GameObject whiteArrowInstance;
    public int test = 0;

    void Start()
    {
        lineRenderer = ManagerScript.instance.lineRenderer;

        Vector3 arrowPosition = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
        whiteArrowInstance = Instantiate(whiteArrow, arrowPosition, Quaternion.identity, transform);

        wsCanvas = GameObject.Find("WorldSpaceCanvas");
        textPosition = new Vector3(transform.position.x, transform.position.y + 0.22f, transform.position.z);
        timerTextInstance = Instantiate(timerText, textPosition, Quaternion.identity, wsCanvas.transform);
    }

    private void FixedUpdate()
    {
        UpdateAngle();
        RunMissileTimer();
    }

    // On mouse down - create a white arrow, set up line renderer, input field, green circle and tell the manager that this launcher is selected.
    private void OnMouseDown()
    {
        if (launcherNumber != ManagerScript.instance.whichSelected && !ManagerScript.instance.hasBegun)
        {
            Thread.Sleep(20);
            DestroySelection();

            ManagerScript.instance.whichSelected = launcherNumber;
            lineRenderer.enabled = true;
            ManagerScript.instance.PlaySound(selection, 1.5f);

            Instantiate(selectionCircle, transform.position, Quaternion.identity);

            if (leftButton == null)
            {
                InitializeButtons(textPosition, wsCanvas);
            }
            
            foreach (GameObject launcher in ManagerScript.instance.launcherArray)
            {
                launcher.GetComponent<RocketLauncherScript>().HideUnhideButtons(false);
            }
            this.HideUnhideButtons(true);

            Vector3 lrPos1 = whiteArrowInstance.transform.position;
            float x3 = whiteArrowInstance.transform.position.x + 2f * Mathf.Cos(missileAngle * Mathf.Deg2Rad);
            float y3 = whiteArrowInstance.transform.position.y + 2f * Mathf.Sin(missileAngle * Mathf.Deg2Rad);
            Vector3 lrPos2 = new Vector3(x3, y3, 0);
            lineRenderer.SetPosition(0, lrPos1);
            lineRenderer.SetPosition(1, lrPos2);
        }

    }

    // Updating the angle for white arrow as well as the line renderer when axis input is detected.
    private void UpdateAngle()
    {
        if (launcherNumber == ManagerScript.instance.whichSelected)
        {

            movement = Input.GetAxisRaw("Horizontal");
            whiteArrowInstance.transform.RotateAround(transform.position, Vector3.forward, movement * Time.fixedDeltaTime* 75f); // Modifiable move speed of the arrow

            float x1 = transform.position.x;
            float y1 = transform.position.y;
            float x2 = whiteArrowInstance.transform.position.x;
            float y2 = whiteArrowInstance.transform.position.y;
            missileAngle = Mathf.Atan2(y2 - y1, x2 - x1) * 180 / Mathf.PI;

            if (previousAngle != missileAngle)
            {
                Vector3 lrPos1 = whiteArrowInstance.transform.position;
                float x3 = whiteArrowInstance.transform.position.x + 2f * Mathf.Cos(missileAngle * Mathf.Deg2Rad);
                float y3 = whiteArrowInstance.transform.position.y + 2f * Mathf.Sin(missileAngle * Mathf.Deg2Rad);
                Vector3 lrPos2 = new Vector3(x3, y3, 0);
                lineRenderer.SetPosition(0, lrPos1);
                lineRenderer.SetPosition(1, lrPos2);

                soundTimer += Time.deltaTime;
                if (soundTimer >= 0.08f)
                {
                    ManagerScript.instance.PlaySound(audioClip, 0.25f);
                    soundTimer = 0f;
                }
            }
            previousAngle = missileAngle;
        }

    }

    // Setting up everything that is necessary when entering 'play mode' (preparing the missile, deactivating the input field)
    public void InitiateLauncher()
    {
        if (timerTextInstance != null)
        {
            missileInstance = Instantiate(missile, transform.position, Quaternion.identity);
            missileInstance.GetComponent<MissileScript>().transform.eulerAngles = new Vector3(0, 0, missileAngle - 90f);
            missileInstance.GetComponent<MissileScript>().angle = missileAngle * Mathf.Deg2Rad;

            HideUnhideButtons(true);
        }

    }

    private void InitializeButtons(Vector3 textPosition, GameObject canvas)
    {
        Vector3 textPositionUp = new Vector3(textPosition.x + 0.15f, textPosition.y, textPosition.z);
        Vector3 textPositionDown = new Vector3(textPosition.x - 0.15f, textPosition.y, textPosition.z);
        rightButton = Instantiate(rightButtonPrefab, textPositionUp, Quaternion.identity, canvas.transform);
        leftButton = Instantiate(leftButtonPrefab, textPositionDown, Quaternion.identity, canvas.transform);
        rightButton.GetComponent<TimerButton>().timerText = timerTextInstance;
        rightButton.GetComponent<TimerButton>().launcherNumber = launcherNumber;
        leftButton.GetComponent<TimerButton>().timerText = timerTextInstance;
        leftButton.GetComponent<TimerButton>().launcherNumber = launcherNumber;
    }

    // Run the timer, after it ends, the missiles are launched.
    private void RunMissileTimer()
    {
        if (ManagerScript.instance.hasBegun)
        {
            missileTimer -= Time.fixedDeltaTime;
            if (missileTimer > 0)
            {
                if (timerTextInstance != null)
                {
                    timerTextInstance.text = missileTimer.ToString("F2");
                }
            }
            else
            {
                if (missileInstance != null)
                {
                    if (whiteArrowInstance != null)
                    {
                        whiteArrowInstance.GetComponent<SpriteRenderer>().enabled = false;
                    }
                    missileInstance.GetComponent<MissileScript>().startBool = true;
                    if (timerTextInstance != null)
                    {
                        timerTextInstance.text = "0";
                    }

                }
            }
        }
    }

    public void HideUnhideButtons(bool which)
    {
        if(leftButton != null)
        {
            leftButton.SetActive(which);
            rightButton.SetActive(which);
        }
        
    }

    // This is was initially more useful as the trash tag included more objects, now it's just to destroy the green circle if I remember correctly.
    private void DestroySelection()
    {
        foreach (GameObject trash in GameObject.FindGameObjectsWithTag("Trash"))
        {
            Destroy(trash);
        }
    }

}
