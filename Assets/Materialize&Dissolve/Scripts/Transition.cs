using Leap.Unity.Infix;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UHI.Tracking.InteractionEngine.Examples.SimpleInteractionGlow;
using static UnityEngine.GraphicsBuffer;

public class Transition : MonoBehaviour
{
    [System.Serializable]
    public class serializableClass
    {
        public List<GameObject> replicaObjects;

    }
    public List<serializableClass> replicaList = new List<serializableClass>();
    public List<serializableClass> target_1_List = new List<serializableClass>();
    public List<serializableClass> replicaList_Room = new List<serializableClass>();
    public List<GameObject> onlyTarget1Objs = new List<GameObject>();
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

    public Light light;
    private float debounceTime = 0.25f;   // Debounce time in seconds
    private bool isButtonClickable = true;  // Flag to keep track of button clickability
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
    bool lastCallToReal = false;
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
                    StartCoroutine(AddReplicaToReplicaAndTarget());
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
                    StartCoroutine(RemoveReplicaOnly());
                    break;

                case TransitionSelector.Translate:
                    StartCoroutine(ReplicaToReal_Translate(3));
                    break;

                default:
                    break;
            }
        }
        if (Input.GetKey(KeyCode.F3))
        {
            switch (currentTransition)
            {
                case TransitionSelector.Fade:
                    StartCoroutine(ReplicaToReal_Fade(3));
                    break;

                case TransitionSelector.Dissolve:
                    StartCoroutine(ReplicaToTarget_1_I_remove_Target1());
                    break;

                case TransitionSelector.Translate:
                    StartCoroutine(ReplicaToReal_Translate(3));
                    break;

                default:
                    break;
            }
        }
        if (Input.GetKey(KeyCode.F4))
        {
            switch (currentTransition)
            {
                case TransitionSelector.Fade:
                    StartCoroutine(ReplicaToReal_Fade(3));
                    break;

                case TransitionSelector.Dissolve:
                    StartCoroutine(AddTargetToTarget());
                    break;

                case TransitionSelector.Translate:
                    StartCoroutine(ReplicaToReal_Translate(3));
                    break;

                default:
                    break;
            }
        }
        if (Input.GetKey(KeyCode.F5) && isButtonClickable) // Replica -> Target
        {
            isButtonClickable = false;
            switch (currentTransition)
            {
                case TransitionSelector.Fade:
                    StartCoroutine(ReplicaToReal_Fade(3));
                    break;

                case TransitionSelector.Dissolve:
                    StartCoroutine(AddReplicaToReplicaAndTarget());                    
                    break;

                case TransitionSelector.Translate:
                    StartCoroutine(ReplicaToReal_Translate(3));
                    break;

                default:
                    break;
            }
            StartCoroutine(EnableButtonAfterDebounce());
        }
        if (Input.GetKey(KeyCode.F6) && isButtonClickable) // Replica -> Target
        {
            isButtonClickable = false;
            switch (currentTransition)
            {
                case TransitionSelector.Fade:
                    StartCoroutine(ReplicaToReal_Fade(3));
                    break;

                case TransitionSelector.Dissolve:
                    StartCoroutine(RemoveTargetToReplica());
                    break;

                case TransitionSelector.Translate:
                    StartCoroutine(ReplicaToReal_Translate(3));
                    break;

                default:
                    break;
            }
            StartCoroutine(EnableButtonAfterDebounce());

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

    private IEnumerator EnableButtonAfterDebounce()
    {
        // Wait for the debounce time
        yield return new WaitForSeconds(debounceTime);

        // Set the button clickability flag to true
        isButtonClickable = true;
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
                    dissolver.ReplaceMaterials();
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
                    dissolver.FadeOut(2.7f);
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
                    dissolver.FadeIn(2.7f);
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


    IEnumerator ReplicaToTarget_1_I_remove_Target1()
    {
        coroutineIsRunning = true;
        int count = 0;
        WaitForSeconds wfs = new WaitForSeconds(0.6f);
        for (int i = target_1_List.Count - 1; i >= 0; i--)
        {
            count++;
            foreach (GameObject replicaObject in target_1_List[i].replicaObjects)
            {
                //Debug.Log(replicaObject.name);
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.Dissolve();
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(ReplicaToTarget_1_I_remove_Target1());
        coroutineIsRunning = false;
    }
    IEnumerator RemoveTargetToReplica()
    {
        coroutineIsRunning = true;
        int count = 0;
        WaitForSeconds wfs = new WaitForSeconds(0.6f);
        foreach (serializableClass target in target_1_List) 
        {
            count++;
            foreach (GameObject replicaObject in target.replicaObjects)
            {
                //Debug.Log(replicaObject.name);
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.Dissolve();
            }
            Debug.Log(count);
            if (count > 6)
            {

                StartCoroutine(AddReplicaToReplica());
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveTargetToReplica());
        coroutineIsRunning = false;
    }
    IEnumerator AddTargetToTarget()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(0.9f);
        for (int i = target_1_List.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in target_1_List[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.Materialize();
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(AddReplicaToReplicaAndTarget());
        coroutineIsRunning = false;
    }
    IEnumerator RemoveReplicaToTarget()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(0.6f);
        int count = 0;
        Debug.Log("Start RemoveReplicaToTarget");
        foreach (serializableClass replica in replicaList)//int i = replicaList.Count - 1; i >= 0; i--
        {
            count++;
            foreach (GameObject replicaObject in replica.replicaObjects)//replicaList[i].replicaObjects
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();

                if (replicaObject.name == "Room")
                {
                    dissolver.Duration = 6.7f;
                }
                else
                {
                    dissolver.Duration = durationPerObject;
                }
                dissolver.Dissolve();
            }
            if(count > 6)
            {
                Debug.Log("Call StartCoroutine(Target_1_ToReplica_I());");
                StartCoroutine(AddTargetToTarget());
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveReplicaToTarget());
        coroutineIsRunning = false;
    }
    IEnumerator RemoveReplicaToReal()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(0.6f);
        int count = 0;
        foreach (serializableClass replica in replicaList)//int i = replicaList.Count - 1; i >= 0; i--
        {
            count++;
            foreach (GameObject replicaObject in replica.replicaObjects)//replicaList[i].replicaObjects
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();

                if (replicaObject.name == "Room")
                {
                    dissolver.Duration = 6.7f;
                }
                else
                {
                    dissolver.Duration = durationPerObject;
                }
                dissolver.Dissolve();
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveReplicaToTarget());
        coroutineIsRunning = false;
    }
    IEnumerator RemoveReplicaOnly()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(0.6f);
        int count = 0;
        foreach (serializableClass replica in replicaList)//int i = replicaList.Count - 1; i >= 0; i--
        {
            count++;
            foreach (GameObject replicaObject in replica.replicaObjects)//replicaList[i].replicaObjects
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();

                if (replicaObject.name == "Room")
                {
                    dissolver.Duration = 6.7f;
                }
                else
                {
                    dissolver.Duration = durationPerObject;
                }
                dissolver.Dissolve();
            }
            yield return wfs;
        }

        Debug.Log("End RemoveReplicaOnly");
        StopCoroutine(RemoveReplicaOnly());
        coroutineIsRunning = false;
    }
    IEnumerator AddReplicaToReplicaAndTarget()
    {
        coroutineIsRunning = true;
        int count = 0;
        WaitForSeconds wfs = new WaitForSeconds(0.6f);
        foreach (serializableClass replica in replicaList)
        {
            count++;
            foreach (GameObject replicaObject in replica.replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.Materialize();
            }
            yield return wfs;

        }
        yield return new WaitForSeconds(2f);
        StartCoroutine(RemoveReplicaToTarget());
        Debug.Log("Call StartCoroutine(RemoveReplicaToTarget())");

        foreach (GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(true);
        }
        Debug.Log("End AddReplicaToReplicaAndTarget");
        StopCoroutine(AddReplicaToReplicaAndTarget());
        coroutineIsRunning = false;
    }
    IEnumerator AddReplicaToReplica()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(0.6f);
        for(int i = replicaList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.Materialize();
            }
            yield return wfs;
        }
        foreach(GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(false);
        }
        Debug.Log("End");
        yield return new WaitForSeconds(2f);
        StartCoroutine(RemoveReplicaToReal()); //hier ende
        
        StopCoroutine(AddReplicaToReplica());

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

