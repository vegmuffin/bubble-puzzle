using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndPanelHighlightScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
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
        ManagerScript.instance.ChangeButtonDescription(description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ManagerScript.instance.ChangeButtonDescription("");
    }
}
