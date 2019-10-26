using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;

public class TimerButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public int launcherNumber;
    [HideInInspector] public Text timerText;

    private int multiplier = 0;
    private RocketLauncherScript rls;
    private float holdTime = 0f;
    private bool holdBool = false;
    private GameObject manager;

    void Start()
    {
        Thread.Sleep(20);
        manager = GameObject.Find("Manager");
        rls = manager.GetComponent<ManagerScript>().launcherArray[launcherNumber].GetComponent<RocketLauncherScript>();
    }

    void Update()
    {
        UpdateTimer();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch(transform.gameObject.name)
        {
            case "RL_TimerTextLeft(Clone)":
                multiplier = -1;
                break;
            case "RL_TimerTextRight(Clone)":
                multiplier = 1;
                break;
        }

        float time = rls.missileTimer;
        if((time != 0f || multiplier != -1) || (time == 0f && multiplier == 1))
        {
            time += 0.01f * multiplier;
            rls.missileTimer = time;
            timerText.text = time.ToString("F2");

            // Make it look satisfying
            transform.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(45, 25);

            holdBool = true;
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        holdBool = false;
        holdTime = 0f;

        transform.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(50, 30);
    }

    private void UpdateTimer()
    {
        if(holdBool)
        {
            holdTime += Time.fixedDeltaTime;
            if(holdTime >= 0.5f) BurstTime();
        }
        
    }

    private void BurstTime()
    {
        float time = rls.missileTimer;
        time += 0.01f * multiplier;
        rls.missileTimer = time;
        timerText.text = time.ToString("F2");

        if(time <= 0f)
        {
            holdBool = false;
            holdTime = 0f;
            rls.missileTimer = 0f;

            transform.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(50, 30);
        }
    }

}
