using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HighlightScript : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private GameObject leftMenuCircle;
    [SerializeField]
    private GameObject rightMenuCircle;
    [SerializeField]
    private Camera mainCamera;
    private GameObject[] buttonArray;
    private Vector3 leftPosition;
    private Vector3 rightPosition;

    public bool isThisSelected = false;
    
    void Start()
    {
        buttonArray = GameObject.FindGameObjectsWithTag("MenuButton");
    }

    void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        leftMenuCircle.transform.Rotate(0, Time.deltaTime * 300f, 0, Space.World);
        rightMenuCircle.transform.Rotate(0, Time.deltaTime * 300f, 0, Space.World);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if(!leftMenuCircle.GetComponent<SpriteRenderer>().enabled || !rightMenuCircle.GetComponent<SpriteRenderer>().enabled)
        {
            leftMenuCircle.GetComponent<SpriteRenderer>().enabled = true;
            rightMenuCircle.GetComponent<SpriteRenderer>().enabled = true;
        }
        if(!isThisSelected)
        {
            foreach(GameObject button in buttonArray)
            {
                button.GetComponent<HighlightScript>().isThisSelected = false;
                button.transform.GetChild(0).GetComponent<Text>().fontSize = 5;
            }
            
            isThisSelected = true;
            leftPosition = new Vector3((transform.GetComponent<RectTransform>().rect.xMin - 4f) / 20, transform.position.y, 10);
            rightPosition = new Vector3((transform.GetComponent<RectTransform>().rect.xMax + 4f) / 20, transform.position.y, 10);
            leftMenuCircle.transform.position = leftPosition;
            rightMenuCircle.transform.position = rightPosition;

            transform.GetChild(0).GetComponent<Text>().fontSize = 4;
        }

    }

}
