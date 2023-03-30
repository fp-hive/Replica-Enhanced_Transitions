using Leap.Unity.Infix;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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

    public GameObject targetBottom;
    public GameObject targetTop;
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
                if (transitionTyp == TransitionSelector.Fade)
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
        Debug.Log("Transition " + transitionTyp.ToString() + " active");

    }

    IEnumerator ReplicaToReal_Fade(float duration)
    {
        WaitForSeconds wfs = new WaitForSeconds(0.6f);

        for (int i = replicaList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && dissolver.name != "Window1")
                {
                    dissolver.FadeOut();
                }

            }
            yield return wfs;
        }
    }
    IEnumerator RealToReplica_Fade(float duration)
    {
        WaitForSeconds wfs = new WaitForSeconds(0.6f);


        for (int i = 0; i <= replicaList.Count; i++)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && dissolver.name != "Window1")
                {
                    dissolver.FadeIn();
                }

            }
            yield return wfs;
        }
    }

    IEnumerator Fade_I()
    {

        yield return null;
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


        WaitForSeconds wfs = new WaitForSeconds(0.6f);


        for (int i = replicaList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null  && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1"))
                {
                    //dissolver.startPosition = replicaObject.transform.position;
                    //dissolver.targetPosition = new Vector3(replicaObject.transform.position.x, replicaObject.transform.position.y+6f, replicaObject.transform.position.z);

                    dissolver.TranslateOut();
                }

            }
            yield return wfs;
        }

    }

    IEnumerator RealToReplica_Translate()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.6f);


        for (int i = 0; i < replicaList.Count; i++)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1"))
                {
                    dissolver.TranslateIn();
                }

            }
            yield return wfs;
        }
    }
}

