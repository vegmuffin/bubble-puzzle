using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IFCustom : MonoBehaviour
{
    [SerializeField]
    private Text timerText;

    public int inputFieldLauncherNumber;
    public Text timerTextInstance;

    // When ending the edit on this input field, we have to parse the input data to the launcher so it could run the numbers. It's also de-selecting the launcher.
    public void OnEndEdit()
    {
        string ifText = transform.GetComponent<InputField>().text;
        if(ifText != "")
        {
            float ifNumber = float.Parse(ifText);
            GameObject manager = GameObject.Find("Manager");
            GameObject launcher = manager.GetComponent<ManagerScript>().launcherArray[inputFieldLauncherNumber];
            launcher.GetComponent<RocketLauncherScript>().missileTimer = ifNumber;

            if(timerTextInstance == null)
            {
                GameObject mainCanvas = GameObject.Find("Canvas");
                Vector3 textPosition = new Vector3(launcher.transform.position.x, launcher.transform.position.y + 0.3f, launcher.transform.position.z);
                launcher.GetComponent<RocketLauncherScript>().staticTextPosition = textPosition;

                textPosition = Camera.main.WorldToScreenPoint(textPosition);
                timerTextInstance = Instantiate(timerText, textPosition, Quaternion.identity, mainCanvas.transform);
                launcher.GetComponent<RocketLauncherScript>().timerTextInstance = timerTextInstance;
            }
            timerTextInstance.text = ifNumber.ToString("F2");

            manager.GetComponent<ManagerScript>().whichSelected = -1;
            transform.gameObject.SetActive(false);
            foreach(GameObject trash in GameObject.FindGameObjectsWithTag("Trash"))
            {
                Destroy(trash);
            }
            manager.GetComponent<ManagerScript>().lineRenderer.enabled = false;
        }
            
    }
}
