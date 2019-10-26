using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScript : MonoBehaviour
{
    public void Click()
    {
        switch(transform.gameObject.name)
        {
            case "Level1":
                SceneManager.LoadScene("Level_1", LoadSceneMode.Single);
                break;
            case "Level2":
                SceneManager.LoadScene("Level_2", LoadSceneMode.Single);
                break;
        }
    }
}
