using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UHI.Tracking.InteractionEngine.Examples.SimpleInteractionGlow;

public class Transition : MonoBehaviour
{
    [System.Serializable]
    public class serializableClass
    {
        public List<GameObject> replicaObjects;
    }
    public List<serializableClass> replicaList = new List<serializableClass>();
    public enum TransitionSelector
    {
        Fade,
        Dissolve,
        Translate
    }

    float durationPerObject = 2.7f;
    float durationStateChange = 4f;
    TransitionSelector currentTransition = TransitionSelector.Fade;
    // Start is called before the first frame update
    void Start()
    {
        num.Add(1);
        num.Add(2);
        num.Add(3);
        num.Add(4);
    }


    private List<int> num = new List<int>();
    bool coroutineIsRunning = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            switch (currentTransition)
            {
                case TransitionSelector.Fade:
                    StartCoroutine(RealToReplica_Fade(3));
                    break;

                    case TransitionSelector.Dissolve:
                    StartCoroutine(RealToReplica_I());
                    break; 

                    case TransitionSelector.Translate:
                    StartCoroutine(RealToReplica_Translate());
                    break;

                    default:
                    break;
            }
        }
        if (Input.GetKey(KeyCode.F2))
        {
            switch (currentTransition)
            {
                case TransitionSelector.Fade:
                    StartCoroutine(ReplicaToReal_Fade(3));
                    break;

                case TransitionSelector.Dissolve:
                    StartCoroutine(ReplicaToReal_I());
                    break;

                case TransitionSelector.Translate:
                    StartCoroutine(ReplicaToReal_Translate(3));
                    break;

                default:
                    break;
            }
        }
        if (Input.GetKeyDown("f"))
        {
            ChangeTransitionTyp(TransitionSelector.Fade);
        }
        if (Input.GetKeyDown("d"))
        {
            ChangeTransitionTyp(TransitionSelector.Dissolve);
        }
        if (Input.GetKeyDown("l"))
        {
            ChangeTransitionTyp(TransitionSelector.Translate);
        }
    }

    private void ChangeTransitionTyp(TransitionSelector transitionTyp)
    {
        currentTransition = transitionTyp;
        foreach (serializableClass replica in replicaList)
        {
            foreach (GameObject replicaObject in replica.replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if(transitionTyp == TransitionSelector.Fade)
                {
                    dissolver.ReplaceMaterials();
                }
                if (transitionTyp == TransitionSelector.Dissolve)
                {
                    dissolver.RestoreDefaultMaterials();
                }
                if (transitionTyp == TransitionSelector.Translate)
                {
                    dissolver.RestoreDefaultMaterials();
                }
            }
        }
        Debug.Log("Transition "+transitionTyp.ToString()+" active");

    }

    IEnumerator ReplicaToReal_Fade(float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            foreach (serializableClass replica in replicaList)
            {
                foreach (GameObject replicaObject in replica.replicaObjects)
                {
                    Renderer dissolver = replicaObject.GetComponent<Renderer>();
                    Material[] mats = dissolver.materials;
                    foreach (Material mat in mats)
                    {
                        if (mat.name != "GlassMat")
                        {
                            Color cs = mat.color;
                            cs.a = cs.a - 0.001f;
                            if (cs.a < 0f)
                            {
                                cs.a = 0f;
                            }
                            mat.color = cs;
                        }
                    }
                    dissolver.materials = mats;
                }
            }
            yield return null;
        }
    }
    IEnumerator RealToReplica_Fade(float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            for (int i = replicaList.Count - 1; i >= 0; i--)
            {
                foreach (GameObject replicaObject in replicaList[i].replicaObjects)
                {
                    if(replicaObject.name == "Window1")
                    {
                        Debug.Log("");

                    }
                    Renderer dissolver = replicaObject.GetComponent<Renderer>();
                    Material[] mats = dissolver.materials;
                    foreach(Material mat in mats)
                    {
                        if(mat.name == "GlassMat (Instance)")
                        {

                            Debug.Log(mat.name.ToString());
                        }
                        else
                        {
                            Color cs = mat.color;
                            cs.a = cs.a + 0.001f;
                            if (cs.a > 1f)
                            {
                                cs.a = 1f;
                            }
                            mat.color = cs;
                        }

                    }

                    dissolver.materials = mats;

                }
            }
            yield return null;
        }
    }


    void ReplicaToTarget1()
    {

    }
    void Target1ToReplica()
    {

    }


    IEnumerator ReplicaToReal_I()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(0.6f);
        for (int i = replicaList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.Dissolve();
            }
            yield return wfs;
        }
        
        Debug.Log("End");
        StopCoroutine(ReplicaToReal_I());
        coroutineIsRunning = false;
    }

    IEnumerator RealToReplica_I()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(0.6f);
        foreach (serializableClass replica in replicaList)
        {
            foreach (GameObject replicaObject in replica.replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.Materialize();
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RealToReplica_I());
        coroutineIsRunning = false;
    }

    IEnumerator ReplicaToReal_Translate(float duration)
    {


        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            foreach (serializableClass replica in replicaList)
            {
                foreach (GameObject replicaObject in replica.replicaObjects)
                {
                    Vector3.MoveTowards(replicaObject.transform.position, new Vector3(replicaObject.transform.position.x + 1f, replicaObject.transform.position.y, replicaObject.transform.position.z), 0.9f);
                }
            }
            yield return null;
        }
    }

    IEnumerator RealToReplica_Translate()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(0.6f);
        foreach (serializableClass replica in replicaList)
        {
            foreach (GameObject replicaObject in replica.replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.Materialize();
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RealToReplica_I());
        coroutineIsRunning = false;
    }
}

