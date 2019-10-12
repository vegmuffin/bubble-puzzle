using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScript : MonoBehaviour
{
    public void Click()
    {
        if(transform.gameObject.name == "Level1")
        {
            SceneManager.LoadScene("Level_1", LoadSceneMode.Single);
        }
    }
}
