using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class MoveCube : MonoBehaviour
{
    Vector3 targetPos = Vector3.zero;
    Vector3 startPos = Vector3.zero;
    float speed = 0.5f;
    float time = 2.7f;
    float controllTime = 0f;
    float a;
    bool isStart = false;
    bool isStop = false;
    
    // Start is called before the first frame update
    void Start()
    {
     
        Debug.Log("A:" + a);
        targetPos.y = this.transform.position.y + 40f;
        startPos.y = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {

     
        controllTime += Time.deltaTime;
       // Debug.Log(controllTime);
        Debug.Log("Time: " + controllTime);
        }

        if (isStop)
        {


            controllTime -= Time.deltaTime;
            // Debug.Log(controllTime);
            Debug.Log("Time: " + controllTime);
        }
        if (Input.GetKey(KeyCode.F5))
          {
            isStart = true;
            StartCoroutine("TranslateOut_I");
            }
        if (Input.GetKey(KeyCode.F6))
        {
            isStop = true;
            StartCoroutine("TranslateIN_I");
        }
    }

    private IEnumerator TranslateOut_I()
    {
        float a = (Vector3.Distance(transform.position, targetPos) * 2.0f) / (time * time);
        float deltaDistance = Vector3.Distance(transform.position, targetPos);
        while (controllTime <= time)
        {
            deltaDistance = Vector3.Distance(transform.position,targetPos);
            Debug.Log("distance: " + deltaDistance);
             Debug.Log("Step Size: " + (0.5f * a * Mathf.Pow(controllTime, 2)));
            Vector3 posTest = new Vector3(transform.position.x, startPos.y + (0.5f * a * Mathf.Pow(controllTime, 2)), transform.position.z);
            this.transform.position = posTest;

            yield return null;
        }
        isStart = false;
    }

    private IEnumerator TranslateIN_I()
    {
        float a = (40f * 2.0f) / (time * time);
        float deltaDistance = Vector3.Distance(transform.position, targetPos);
        while (controllTime >= 0f)// && controllTime < 4f)
        {
            deltaDistance = Mathf.Abs(transform.position.y - startPos.y);

            Vector3 posTest = new Vector3(transform.position.x, startPos.y + (0.5f * a * Mathf.Pow(controllTime, 2)), transform.position.z);
            this.transform.position = posTest;
            yield return null;
        }
        isStop = false;
    }
}
