using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerScript : MonoBehaviour
{
    private Vector3 staticCameraPosition;
    [SerializeField]
    private GameObject bubble;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private ParticleSystem starParticleSystem;
    [SerializeField]
    private GameObject endPanel;
    [SerializeField]
    private LineRenderer guideline;
    private ParticleSystem particleSystemInstance;
    private float boxMinX;
    private float boxMinY;
    private float boxMaxX;
    private float boxMaxY;
    private bool particleSystemBool = true;
    private float particleSystemTimer = 0f;
    private float globalTimer = 0f;
    private bool timerBool = false;
    private bool panelBool = false;
    private float panelAlpha = 0f;

    public GameObject[] launcherArray;
    public LineRenderer lineRenderer;
    public int whichSelected = -1;
    public bool hasBegun = false;
    public bool isInTheBox = false;
    public Text globalTimerText;

    // Gathering all the launchers and taking the current camera position.
    void Start()
    {
        launcherArray = GameObject.FindGameObjectsWithTag("Launcher");
        for(int i = 0; i < launcherArray.Length; ++i)
        {
            launcherArray[i].GetComponent<RocketLauncherScript>().launcherNumber = i;
        }
        staticCameraPosition = mainCamera.transform.position;

        Bounds bounds = GameObject.Find("EndBox").GetComponent<BoxCollider2D>().bounds;
        boxMinX = bounds.min.x;
        boxMaxX = bounds.max.x;
        boxMinY = bounds.min.y;
        boxMaxY = bounds.max.y;
    }

    void Update()
    {
        MoveCamera();
        CheckBoxStatus();
        UpdateParticleSystemTimer();
        UpdateTimer();
        PopEndPanel();
        DestroySelection();
    }

    // When in 'play mode', camera is focused on the bubble.
    private void MoveCamera()
    {
        if(hasBegun)
        {
            mainCamera.transform.position = new Vector3(bubble.transform.position.x, bubble.transform.position.y, -10);
        }
    }

    private void DestroySelection()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            foreach(GameObject trash in GameObject.FindGameObjectsWithTag("Trash"))
            {
                Destroy(trash);
            }
            foreach(GameObject launcher in launcherArray)
            {
                if(launcher.GetComponent<RocketLauncherScript>().launcherNumber == whichSelected)
                {
                    launcher.GetComponent<RocketLauncherScript>().inputFieldInstance.transform.gameObject.SetActive(false);
                }
            }
            lineRenderer.enabled = false;
            whichSelected = -1;
        }
    }

    // Checking if the bubble is in the box.
    private void CheckBoxStatus()
    {
        if(hasBegun && !isInTheBox)
        {
            if(bubble.transform.position.x > boxMinX && bubble.transform.position.y > boxMinY && bubble.transform.position.x < boxMaxX && bubble.transform.position.y < boxMaxY)
            {
                Vector3 particleSystemPosition = new Vector3((boxMinX+boxMaxX)/2, boxMinY + 0.2f, 10);
                particleSystemInstance = Instantiate(starParticleSystem, particleSystemPosition, Quaternion.identity);
                particleSystemInstance.transform.eulerAngles = new Vector3(-90f, 0, 0);
                isInTheBox = true;

                timerBool = false;
                GameObject bb = GameObject.Find("BeginButton");
                GameObject mb = GameObject.Find("ManageButton");
                bb.GetComponent<Button>().interactable = false;
                mb.GetComponent<Button>().interactable = false;
            }
        }
    }

    // Updating Particle System timer so we could destroy it later.
    private void UpdateParticleSystemTimer()
    {
        if(particleSystemBool && isInTheBox)
        {
            if(particleSystemTimer <= 1.75f)
            {
                particleSystemTimer += Time.deltaTime;
            } else if(particleSystemTimer <= 4f && particleSystemTimer >= 1.75f)
            {
                particleSystemTimer += Time.deltaTime;
                particleSystemInstance.Stop();
                if(!panelBool && panelAlpha == 0f)
                {
                    panelBool = true;
                    endPanel.SetActive(true);
                    for(int i = 0; i < endPanel.transform.childCount; ++i)
                    {
                        endPanel.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            } else
            {
                particleSystemTimer = 0f;
                Destroy(particleSystemInstance.gameObject);
                particleSystemBool = false;
            }
        }
    }

    private void UpdateTimer()
    {
        if(timerBool)
        {
            globalTimer += Time.deltaTime;
            globalTimerText.text = "Time: " + globalTimer.ToString("F1");
        }
    }

    // Begin button functionality (destroy trash objects, disable/enable buttons, etc.)
    public void Begin(Button btn)
    {
        bool isEverythingOk = true;
        foreach(GameObject launcher in launcherArray)
        {
            if(launcher.GetComponent<RocketLauncherScript>().timerTextInstance == null)
            {
                isEverythingOk = false;
                break;
            }
        }
        if(isEverythingOk)
        {
            hasBegun = true;
            whichSelected = -1;
            GameObject[] trashArray = GameObject.FindGameObjectsWithTag("Trash");
            foreach(GameObject trash in trashArray)
            {
                Destroy(trash);
            }

            foreach(GameObject launcher in launcherArray)
            {
                launcher.GetComponent<RocketLauncherScript>().InitiateLauncher();
            }

            btn.interactable = false;
            GameObject.Find("ManageButton").GetComponent<Button>().interactable = true;
            bubble.GetComponent<TrailRenderer>().enabled = true;
            bubble.GetComponent<BubbleScript>().guidelinePoints.Clear();
            guideline.gameObject.SetActive(false);
            if(lineRenderer.enabled == true)
            {
                lineRenderer.enabled = false;
            }
            timerBool = true;
        }
        
    }

    // Manage button functionality (refresh text objects, destroy missiles, basically restart the level)
    public void Manage(Button btn)
    {
        if(hasBegun)
        {
            hasBegun = false;
            Camera.main.transform.position = staticCameraPosition;
            foreach(GameObject launcher in launcherArray)
            {
                launcher.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                launcher.GetComponent<RocketLauncherScript>().timerTextInstance.text = launcher.GetComponent<RocketLauncherScript>().staticMissileTimer.ToString();
                launcher.GetComponent<RocketLauncherScript>().missileTimer = launcher.GetComponent<RocketLauncherScript>().staticMissileTimer;

                launcher.GetComponent<RocketLauncherScript>().timerTextInstance.transform.position = mainCamera.WorldToScreenPoint(launcher.GetComponent<RocketLauncherScript>().staticTextPosition);
            }
            foreach(GameObject missile in GameObject.FindGameObjectsWithTag("Missile"))
            {
                Destroy(missile.gameObject);
            }
            btn.interactable = false;
            GameObject.Find("BeginButton").GetComponent<Button>().interactable = true;

            GameObject bubble = GameObject.Find("Bubble");
            bubble.transform.position = bubble.GetComponent<BubbleScript>().startingPosition;
            bubble.GetComponent<TrailRenderer>().enabled = false;
            bubble.GetComponent<ConstantForce2D>().force = Vector2.zero;
            bubble.GetComponent<Rigidbody2D>().gravityScale = 0;
            bubble.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            bubble.transform.eulerAngles = Vector3.zero;
            bubble.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            bubble.GetComponent<BubbleScript>().glass.GetComponent<SpriteRenderer>().enabled = true;
            bubble.GetComponent<BubbleScript>().isInGlass = true;

            timerBool = false;
            globalTimer = 0f;
            globalTimerText.text = "Time: " + globalTimer.ToString("F1");

            if(bubble.GetComponent<BubbleScript>().guidelinePoints.Count != 0)
            {
                guideline.gameObject.SetActive(true);
                List<Vector3> guidelinePoints = bubble.GetComponent<BubbleScript>().guidelinePoints;
                guideline.positionCount = guidelinePoints.Count;
                for(int i = 0; i < guidelinePoints.Count; ++i)
                {
                    guideline.SetPosition(i, guidelinePoints[i]);
                }
            }
        }
    }

    public void NextLevel()
    {

    }

    public void MainMenu()
    {
        
    }

    private void PopEndPanel()
    {
        if(panelBool && panelAlpha < 0.7f)
        {
            panelAlpha += 0.015f;
            Color tempColor = endPanel.GetComponent<Image>().color;
            tempColor.a = panelAlpha;
            endPanel.GetComponent<Image>().color = tempColor;
        } else if(panelBool && panelAlpha >= 0.7f)
        {
            panelBool = false;
            for(int i = 0; i < endPanel.transform.childCount; ++i)
            {
                endPanel.transform.GetChild(i).gameObject.SetActive(true);
                if(i == 2)
                {
                    endPanel.transform.GetChild(i).GetComponent<Text>().text = "TIME: " + globalTimer.ToString("F1");
                }
            }
        }       
    }
}
