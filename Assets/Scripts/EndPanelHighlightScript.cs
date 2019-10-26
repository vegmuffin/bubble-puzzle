using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndPanelHighlightScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private ManagerScript ms;

    void Start()
    {
        ms = GameObject.Find("Manager").GetComponent<ManagerScript>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string description = "";
        if(transform.gameObject.name == "NextLevelButton")
        {
            description = "NEXT LEVEL";
        } else if(transform.gameObject.name == "ReplayButton")
        {
            description = "REPLAY";
        } else
        {
            description = "MAIN MENU";
        }
        ms.ChangeButtonDescription(description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ms.ChangeButtonDescription("");
    }
}
