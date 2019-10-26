using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManagerScript : MonoBehaviour
{
    private Vector3 staticCameraPosition;
    [SerializeField] private GameObject bubble;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ParticleSystem starParticleSystem;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private LineRenderer guideline;
    [SerializeField] private GameObject explosionIndicator;
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
    private bool pauseBool = false;
    private Text buttonDescription;
    private List<GameObject> explosionIndicators = new List<GameObject>();

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
                    launcher.GetComponent<RocketLauncherScript>().HideUnhideButtons(false);
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
                hasBegun = false;
                particleSystemBool = true;

                timerBool = false;
                GameObject.Find("BeginButton").GetComponent<Button>().interactable = false;
                GameObject.Find("ManageButton").GetComponent<Button>().interactable = false;
                GameObject.Find("PauseButton").GetComponent<Button>().interactable = false;
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
                particleSystemTimer += Time.fixedDeltaTime;
            } else if(particleSystemTimer <= 4f && particleSystemTimer >= 1.75f)
            {
                particleSystemTimer += Time.fixedDeltaTime;
                particleSystemInstance.Stop();
                if(!panelBool && panelAlpha == 0f)
                {
                    panelBool = true;
                    endPanel.SetActive(true);
                    float screenAspect = (float)Screen.width / (float)Screen.height;
                    float cameraHeight = mainCamera.orthographicSize * 2;
                    Bounds bounds = new Bounds(
                        mainCamera.transform.position,
                        new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
                    RectTransform rTransform = endPanel.GetComponent<Image>().rectTransform;
                    rTransform.sizeDelta = new Vector2(bounds.size.x - 100, rTransform.sizeDelta.y);
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
            globalTimer += Time.fixedDeltaTime;
            globalTimerText.text = "Time: " + globalTimer.ToString("F1");
        }
    }

    // Begin button functionality (destroy trash objects, disable/enable buttons, etc.)
    public void Begin(Button btn)
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
            RocketLauncherScript rls = launcher.GetComponent<RocketLauncherScript>();
            rls.staticMissileTimer = rls.missileTimer;
            rls.InitiateLauncher();
            rls.HideUnhideButtons(false);
        }

        btn.interactable = false;
        GameObject.Find("ManageButton").GetComponent<Button>().interactable = true;
        GameObject.Find("PauseButton").GetComponent<Button>().interactable = true;
        pauseBool = false;
        bubble.GetComponent<TrailRenderer>().enabled = true;
        bubble.GetComponent<BubbleScript>().guidelinePoints.Clear();
        guideline.gameObject.SetActive(false);
        if(lineRenderer.enabled == true)
        {
            lineRenderer.enabled = false;
        }
        timerBool = true;
        
        for(int i = 0; i < explosionIndicators.Count; ++i)
        {
            Destroy(explosionIndicators[i]);
        }
        explosionIndicators.Clear();
        bubble.GetComponent<BubbleScript>().explosionPoints.Clear();
    }

    // Manage button functionality (refresh text objects, destroy missiles, basically restart the level)
    public void Manage(Button btn)
    {
        if(hasBegun)
        {
            hasBegun = false;
            Button pause = GameObject.Find("PauseButton").GetComponent<Button>();
            if(pause.transform.GetChild(0).GetComponent<Text>().text == "unpause")
            {
                Time.timeScale = 1;
                pause.transform.GetChild(0).GetComponent<Text>().text = "pause";
            }
            Camera.main.transform.position = staticCameraPosition;
            foreach(GameObject launcher in launcherArray)
            {
                launcher.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                launcher.GetComponent<RocketLauncherScript>().timerTextInstance.text = launcher.GetComponent<RocketLauncherScript>().staticMissileTimer.ToString("F2");
                launcher.GetComponent<RocketLauncherScript>().missileTimer = launcher.GetComponent<RocketLauncherScript>().staticMissileTimer;
                launcher.GetComponent<RocketLauncherScript>().test = 0;
            }
            foreach(GameObject missile in GameObject.FindGameObjectsWithTag("Missile"))
            {
                Destroy(missile.gameObject);
            }
            btn.interactable = false;
            GameObject.Find("BeginButton").GetComponent<Button>().interactable = true;
            pause.interactable = false;

            GameObject bubble = GameObject.Find("Bubble");
            BubbleScript bs = bubble.GetComponent<BubbleScript>();
            Rigidbody2D brb = bubble.GetComponent<Rigidbody2D>();
            bubble.transform.position = bubble.GetComponent<BubbleScript>().startingPosition;
            bubble.GetComponent<TrailRenderer>().enabled = false;
            bubble.GetComponent<ConstantForce2D>().force = Vector2.zero;
            brb.gravityScale = 0;
            brb.velocity = Vector3.zero;
            bubble.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,0f));
            brb.angularVelocity = 0f;
            brb.Sleep();
            bs.glass.GetComponent<SpriteRenderer>().enabled = true;
            bs.ResetSprite();
            bs.isInGlass = true;

            timerBool = false;
            globalTimer = 0f;
            globalTimerText.text = "Time: " + globalTimer.ToString("F1");

            if(bs.guidelinePoints.Count != 0)
            {
                guideline.gameObject.SetActive(true);
                List<Vector3> guidelinePoints = bs.guidelinePoints;
                guideline.positionCount = guidelinePoints.Count;
                for(int i = 0; i < guidelinePoints.Count; ++i)
                {
                    guideline.SetPosition(i, guidelinePoints[i]);
                }
            }
            if(bs.explosionPoints.Count != 0)
            {
                for(int i = 0; i < bs.explosionPoints.Count; ++i)
                {
                    GameObject explosion = Instantiate(explosionIndicator, bs.explosionPoints[i], Quaternion.identity);
                    explosionIndicators.Add(explosion);
                }
            }
        }
    }

    public void Pause()
    {
        Button pause = GameObject.Find("PauseButton").GetComponent<Button>();
        if(pauseBool)
        {
            Time.timeScale = 1;
            pause.transform.GetChild(0).GetComponent<Text>().text = "pause";
        } else
        {
            Time.timeScale = 0;
            pause.transform.GetChild(0).GetComponent<Text>().text = "unpause";
        }
        pauseBool = !pauseBool;
    }

    public void NextLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string levelNumberStr = sceneName.Substring(6);
        int levelNumber = int.Parse(levelNumberStr);
        ++levelNumber;
        string newLevelString = "Level_" + levelNumber.ToString();
        SceneManager.LoadScene(newLevelString);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
                if(i == 4 && buttonDescription == null)
                {
                    buttonDescription = endPanel.transform.GetChild(i).GetComponent<Text>();
                    buttonDescription.transform.position = new Vector3(buttonDescription.transform.position.x, endPanel.transform.GetComponent<RectTransform>().rect.max.y + 315f, buttonDescription.transform.position.z);
                }
            }
        }       
    }

    public void ChangeButtonDescription(string text)
    {
        buttonDescription.text = text;
    }

    public void PlaySound(AudioClip audioClip, float volume)
    {
        transform.GetComponent<AudioSource>().PlayOneShot(audioClip, volume);
    }
}
