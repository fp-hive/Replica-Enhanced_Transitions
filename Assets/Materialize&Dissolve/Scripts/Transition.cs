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

    float durationPerObject = 1.5f;//2.7f
    float durationStateChange = 4f;//
    float durationRoom = 3f;//6f
    float waitToFinischCoroutine = 2f;//2f
    float durationNextObj = 0.4f;//0.6f
    public GameObject targetBottom;
    public GameObject targetTop;
    TransitionSelector currentTransition = TransitionSelector.Fade;

    public Light light;
    public bool testWithVarjo = false;
    private float debounceTime = 0.25f;   // Debounce time in seconds
    private bool isButtonClickable = true;  // Flag to keep track of button clickability
    // Start is called before the first frame update
    void Start()
    {
        if (true)
        {
            durationPerObject = 1.5f;
            durationRoom = 3f;
            waitToFinischCoroutine = 2f;
            durationNextObj = 0.4f;
        }
        if (false)
        {
            durationPerObject = 1.5f;
            durationRoom = 3f;
            waitToFinischCoroutine = 2f;
            durationNextObj = 0.4f;
        }
        num.Add(1);
        num.Add(2);
        num.Add(3);
        num.Add(4);
        ChangeTransitionTyp(TransitionSelector.Translate);
        //StartCoroutine(RemoveTargetToReplica_Dissolve());
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
                    StartCoroutine(AddReplicaToReplicaAndTarget_Dissolve());
                    break;

                case TransitionSelector.Translate:
                    StartCoroutine(AddReplicaToReplicaAndTarget_Translate());
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
                    StartCoroutine(RemoveReplicaOnly_Translate());
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
                    StartCoroutine(AddTargetToTarget_Translate());
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
                    StartCoroutine(AddTargetToTarget_Dissolve());
                    break;

                case TransitionSelector.Translate:
                    StartCoroutine(RemoveTargetToReplica_Translate());
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
                    StartCoroutine(AddReplicaToReplicaAndTarget_Fade());
                    break;

                case TransitionSelector.Dissolve:
                    StartCoroutine(AddReplicaToReplicaAndTarget_Dissolve());                    
                    break;

                case TransitionSelector.Translate:
                    StartCoroutine(AddReplicaToReplicaAndTarget_Translate());
                    break;

                default:
                    break;
            }
            StartCoroutine(EnableButtonAfterDebounce());
        }
        if (Input.GetKey(KeyCode.F6) && isButtonClickable) //  Target -> Replica
        {
            isButtonClickable = false;
            switch (currentTransition)
            {
                case TransitionSelector.Fade:
                    StartCoroutine(RemoveTargetToReplica_Fade());
                    break;

                case TransitionSelector.Dissolve:
                    StartCoroutine(RemoveTargetToReplica_Dissolve());
                    break;

                case TransitionSelector.Translate:
                    StartCoroutine(RemoveTargetToReplica_Translate());
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
        foreach (serializableClass target in target_1_List)
        {
            foreach (GameObject targetObject in target.replicaObjects)
            {
                Dissolver dissolver = targetObject.GetComponent<Dissolver>();
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
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);

        for (int i = replicaList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null)
                {
                    dissolver.FadeOut(2.7f);
                }

            }
            yield return wfs;
        }
    }
    IEnumerator RealToReplica_Fade(float duration)
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);


        for (int i = 0; i <= replicaList.Count; i++)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null)
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
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
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
    IEnumerator RemoveTargetToReplica_Dissolve()
    {
        coroutineIsRunning = true;
        int count = 0;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
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
                StartCoroutine(AddReplicaToReplica_Dissolve());
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveTargetToReplica_Dissolve());
        coroutineIsRunning = false;
    }
    IEnumerator RemoveTargetToReplica_Fade()
    {
        coroutineIsRunning = true;
        int count = 0;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        foreach (serializableClass target in target_1_List)
        {
            count++;
            foreach (GameObject replicaObject in target.replicaObjects)
            {
                //Debug.Log(replicaObject.name);
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.FadeOut(durationPerObject);
            }
            Debug.Log(count);
            if (count > 6)
            {
                StartCoroutine(AddReplicaToReplica_Fade());
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveTargetToReplica_Dissolve());
        coroutineIsRunning = false;
    }
    IEnumerator AddTargetToTarget_Dissolve()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
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
        StopCoroutine(AddReplicaToReplicaAndTarget_Dissolve());
        coroutineIsRunning = false;
    }
    IEnumerator AddTargetToTarget_Fade()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        for (int i = target_1_List.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in target_1_List[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.FadeIn(durationPerObject);
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(AddReplicaToReplicaAndTarget_Dissolve());
        coroutineIsRunning = false;
    }
    IEnumerator RemoveReplicaToTarget_Dissolve()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
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
                    dissolver.Duration = durationRoom;
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
                StartCoroutine(AddTargetToTarget_Dissolve());
                if (testWithVarjo) { Core.XRSceneManager.Instance.arVRToggle.SetModeToVR(); }
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveReplicaToTarget_Dissolve());
        coroutineIsRunning = false;
    }
    IEnumerator RemoveReplicaToTarget_Fade()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        Debug.Log("Start RemoveReplicaToTarget_Fade");
        foreach (serializableClass replica in replicaList)//int i = replicaList.Count - 1; i >= 0; i--
        {
            count++;
            foreach (GameObject replicaObject in replica.replicaObjects)//replicaList[i].replicaObjects
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();

                if (replicaObject.name == "Room")
                {
                    dissolver.Duration = durationRoom;
                }
                else
                {
                    dissolver.Duration = durationPerObject;
                }
                dissolver.FadeOut(durationPerObject);
            }
            if (count > 6)
            {
                Debug.Log("Call StartCoroutine(Target_1_ToReplica_I());");
                StartCoroutine(AddTargetToTarget_Fade());
                if (testWithVarjo) { Core.XRSceneManager.Instance.arVRToggle.SetModeToVR(); }
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveReplicaToTarget_Fade());
        coroutineIsRunning = false;
    }
    IEnumerator RemoveReplicaToReal_Dissolve()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        for (int i = replicaList.Count - 1; i >= 0; i--)//int i = replicaList.Count - 1; i >= 0; i--
        {
            count++;
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)//replicaList[i].replicaObjects
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();

                if (replicaObject.name == "Room")
                {
                    dissolver.Duration = durationRoom;//
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
        StopCoroutine(RemoveReplicaToTarget_Dissolve());
        coroutineIsRunning = false;
    }
    IEnumerator RemoveReplicaToReal_Fade()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        for (int i = replicaList.Count - 1; i >= 0; i--)//int i = replicaList.Count - 1; i >= 0; i--
        {
            count++;
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)//replicaList[i].replicaObjects
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();

                if (replicaObject.name == "Room")
                {
                    dissolver.Duration = durationRoom;//
                }
                else
                {
                    dissolver.Duration = durationPerObject;
                }
                dissolver.FadeOut(durationPerObject);
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveReplicaToTarget_Dissolve());
        coroutineIsRunning = false;
    }
    IEnumerator RemoveReplicaOnly()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        foreach (serializableClass replica in replicaList)//int i = replicaList.Count - 1; i >= 0; i--
        {
            count++;
            foreach (GameObject replicaObject in replica.replicaObjects)//replicaList[i].replicaObjects
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();

                if (replicaObject.name == "Room")
                {
                    dissolver.Duration = durationRoom;//6
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
    IEnumerator AddReplicaToReplicaAndTarget_Dissolve()
    {
        coroutineIsRunning = true;
        int count = 0;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
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
        yield return new WaitForSeconds(waitToFinischCoroutine);
        StartCoroutine(RemoveReplicaToTarget_Dissolve());
        Debug.Log("Call StartCoroutine(RemoveReplicaToTarget())");

        foreach (GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(true);
        }
        Debug.Log("End AddReplicaToReplicaAndTarget_Dissolve");
        StopCoroutine(AddReplicaToReplicaAndTarget_Dissolve());
        coroutineIsRunning = false;
    }
    IEnumerator AddReplicaToReplicaAndTarget_Fade()
    {
        coroutineIsRunning = true;
        int count = 0;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        foreach (serializableClass replica in replicaList)
        {
            count++;
            foreach (GameObject replicaObject in replica.replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.FadeIn(durationPerObject);
            }
            yield return wfs;

        }
        yield return new WaitForSeconds(waitToFinischCoroutine);
        StartCoroutine(RemoveReplicaToTarget_Fade());
        Debug.Log("Call RemoveReplicaToTarget_Fade");

        foreach (GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(true);
        }
        Debug.Log("End AddReplicaToReplicaAndTarget_Fade");
        StopCoroutine(AddReplicaToReplicaAndTarget_Fade());
        coroutineIsRunning = false;
    }
    IEnumerator AddReplicaToReplica_Dissolve()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
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
        if (testWithVarjo)
        {
            Core.XRSceneManager.Instance.arVRToggle.SetModeToAR();
        }

        yield return new WaitForSeconds(waitToFinischCoroutine);
        StartCoroutine(RemoveReplicaToReal_Dissolve()); //hier ende
        
        StopCoroutine(AddReplicaToReplica_Dissolve());

        coroutineIsRunning = false;
    }
    IEnumerator AddReplicaToReplica_Fade()
    {
        coroutineIsRunning = true;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        for (int i = replicaList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.FadeIn(durationPerObject);
            }
            yield return wfs;
        }
        foreach (GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(false);
        }
        Debug.Log("End");
        if (testWithVarjo)
        {
            Core.XRSceneManager.Instance.arVRToggle.SetModeToAR();
        }

        yield return new WaitForSeconds(waitToFinischCoroutine);
        StartCoroutine(RemoveReplicaToReal_Fade()); //hier ende

        StopCoroutine(AddReplicaToReplica_Fade());

        coroutineIsRunning = false;
    }

    IEnumerator RemoveReplicaToReal_Translate()
    {


        WaitForSeconds wfs = new WaitForSeconds(0.6f);
        int count = 0;

        for (int i = replicaList.Count - 1; i >= 0; i--)
        {
            count++;
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null  && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    //dissolver.startPosition = replicaObject.transform.position;
                    //dissolver.targetPosition = new Vector3(replicaObject.transform.position.x, replicaObject.transform.position.y+6f, replicaObject.transform.position.z);

                    dissolver.TranslateOut();
                }

            }
            if (count > 4)
            {
                count=0;
                StartCoroutine(AddTargetToTarget_Translate());
            }
            yield return wfs;
        }
    }
    IEnumerator RemoveReplicaToTarget_Translate()
    {


        WaitForSeconds wfs = new WaitForSeconds(0.6f);
        int count = 0;

        foreach (serializableClass replica in replicaList)
        {
            count++;
            foreach (GameObject replicaObject in replica.replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    //dissolver.startPosition = replicaObject.transform.position;
                    //dissolver.targetPosition = new Vector3(replicaObject.transform.position.x, replicaObject.transform.position.y+6f, replicaObject.transform.position.z);

                    dissolver.TranslateOut();
                }

            }
            if (count > 4)
            {
                count = 0;
                StartCoroutine(AddTargetToTarget_Translate());
            }
            yield return wfs;
        }
    }
    IEnumerator RemoveReplicaOnly_Translate()
    {


        WaitForSeconds wfs = new WaitForSeconds(0.6f);


        for (int i = replicaList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    //dissolver.startPosition = replicaObject.transform.position;
                    //dissolver.targetPosition = new Vector3(replicaObject.transform.position.x, replicaObject.transform.position.y+6f, replicaObject.transform.position.z);

                    dissolver.TranslateOut();
                }

            }
            yield return wfs;
        }
    }
    IEnumerator RemoveTargetToReplica_Translate()
    {


        WaitForSeconds wfs = new WaitForSeconds(0.6f);

        int count = 0;
        for (int i = target_1_List.Count - 1; i >= 0; i--)
        {
            count++;
            foreach (GameObject replicaObject in target_1_List[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null)// && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    //dissolver.startPosition = replicaObject.transform.position;
                    //dissolver.targetPosition = new Vector3(replicaObject.transform.position.x, replicaObject.transform.position.y+6f, replicaObject.transform.position.z);

                    dissolver.TranslateOut();
                }

            }
            if (count >2)
            {
                count = 0;
                StartCoroutine(AddReplicaToReplica_Translate());
            }
            yield return wfs;
        }

    }
    IEnumerator AddReplicaToReplica_Translate()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.6f);


        for (int i = replicaList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateIn();
                }

            }
            yield return wfs;
        }
        //Hier F2
        foreach(GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(false);
        }
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(RemoveReplicaOnly_Translate());
    }

    IEnumerator AddReplicaToReplicaAndTarget_Translate()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.6f);


        for (int i = 0; i < replicaList.Count; i++)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateIn();
                }

            }
            yield return wfs;
        }
        //Hier F2
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(RemoveReplicaToTarget_Translate());
        foreach (GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(true);
        }
    }
    IEnumerator AddTargetToTarget_Translate()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.6f);


        for (int i = 0; i < target_1_List.Count; i++)
        {
            foreach (GameObject replicaObject in target_1_List[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null)// && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateIn();
                }

            }
            yield return wfs;
        }
    }
}

