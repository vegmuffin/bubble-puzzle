using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    private GameObject[] buttonArray;
    // Start is called before the first frame update
    void Start()
    {
        buttonArray = GameObject.FindGameObjectsWithTag("MenuButton");
    }
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Return))
        {
            Click();
        }
    }

    public void Click()
    {
        GameObject selected = null;
        foreach(GameObject button in buttonArray)
        {
            if(button.GetComponent<HighlightScript>().isThisSelected)
            {
                selected = button;
                break;
            }
        }
        if(selected != null)
        {
            if(selected.name == "LMenu Button")
            {
                SceneManager.LoadScene("LevelMenu", LoadSceneMode.Single);
            }

            if(selected.name == "Quit Button")
            {
                Application.Quit();
            }
        }
        
    }
}
