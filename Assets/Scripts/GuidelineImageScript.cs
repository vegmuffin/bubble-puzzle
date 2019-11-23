using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidelineImageScript : MonoBehaviour
{
    public GameObject explosion;
    public GameObject trampoline;

    public List<Vector3> explosionPoints = new List<Vector3>();
    public List<GameObject> explosionIndicators = new List<GameObject>();
    public List<Vector3> trampolinePoints = new List<Vector3>();
    public List<GameObject> trampolineIndicators = new List<GameObject>();
}
