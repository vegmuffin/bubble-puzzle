using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [HideInInspector] public float timer = 0.5f;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator ShakeCamera()
    {
        while(true)
        {
            timer -= Time.deltaTime;
            if(timer > 0f)
            {
                float randomX = Random.Range(-0.005f, 0.005f);
                float randomY = Random.Range(-0.005f, 0.005f);
                transform.position = new Vector2(randomX, randomY);
                yield return new WaitForSeconds(0.005f);
            } else
            {
                timer = 0.5f;
                transform.position = Vector2.zero;
                yield break;
            }
        }
            
    }

}
