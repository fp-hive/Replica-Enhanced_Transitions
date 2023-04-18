using Leap.Unity.Infix;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static Transition;
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

    private List<string> replicaTranslateStringList = new List<string>();
    public List<GameObject> replicaTranslateList = new List<GameObject>();
    private List<string> replicaFadeStringList = new List<string>();
    public List<GameObject> replicaFadeList = new List<GameObject>();
    public enum TransitionSelector
    {
        Fade,
        Dissolve,
        Translate,
        Combine
    }

    float durationPerObject = 1.5f;//2.7f
    float durationStateChange = 4f;//
    float durationRoom = 3f;//6f
    float waitToFinischCoroutine = 2f;//2f
    float durationNextObj = 0.4f;//0.6f
    public GameObject targetBottom;
    public GameObject targetTop;
    TransitionSelector currentTransition = TransitionSelector.Dissolve;

    public Light light;
    public bool testWithVarjo = false;
    private float debounceTime = 0.25f;   // Debounce time in seconds
    private bool isButtonClickable = true;  // Flag to keep track of button clickability

    private float stopWatchStart = 0.0f;
    private float stopWatchEnd = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (false)
        {
            durationPerObject = 2.0f;           // Dissolve 16-18   Translate 17-18   Fade 15-16
            durationRoom = 3.5f;
            waitToFinischCoroutine = 2.5f;
            durationNextObj = 0.5f;
        }
        if (false)
        {
            durationPerObject = 2.0f;           // Dissolve 11-12  Translate 11-12   Fade 11-12
            durationRoom = 3.5f;
            waitToFinischCoroutine = 2.5f;
            durationNextObj = 0.3f;
        }
        if (true)
        {
            durationPerObject = 1.5f;           // Dissolve 11-12  Translate 11-12   Fade 11-12
            durationRoom = 3.5f;
            waitToFinischCoroutine = 2.5f;
            durationNextObj = 0.3f;
        }
        num.Add(1);
        num.Add(2);
        num.Add(3);
        num.Add(4);
        //ChangeTransitionTyp(TransitionSelector.Translate);
        //StartCoroutine(RemoveTargetToReplica_Dissolve());
        foreach(GameObject obj in replicaTranslateList)
        {
            replicaTranslateStringList.Add(obj.name);
        }
        foreach (GameObject obj in replicaFadeList)
        {
            replicaFadeStringList.Add(obj.name);
        }
    }


    private List<int> num = new List<int>();
    bool coroutineIsRunning_AddReplicaToReplica_Translate = false;
    bool coroutineIsRunning_RemoveReplicaToTarget_Translate = false;
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
            stopWatchStart = Time.time;
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
                case TransitionSelector.Combine:
                    StartCoroutine(AddReplicaToReplicaAndTarget_Combine());
                    break;
                default:
                    break;
            }
            StartCoroutine(EnableButtonAfterDebounce());
        }
        if (Input.GetKey(KeyCode.F6) && isButtonClickable) //  Target -> Replica
        {
            stopWatchStart = Time.time;
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
        if (Input.GetKeyDown("c"))
        {
            ChangeTransitionTyp(TransitionSelector.Combine);
        }
        if (Input.GetKeyDown("r"))
        {
            ResetForCombine();
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
        if( currentTransition == TransitionSelector.Translate)
        {
            ResetTranslate();
        }
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
                if (transitionTyp == TransitionSelector.Combine)
                {
                    if(dissolver.name == "rollo1" || dissolver.name == "Jalusie" || dissolver.name == "rollo2")
                    {
                        dissolver.ReplaceMaterials();
                    }
                    else if (dissolver.name == "Room" || dissolver.name == "Roof") {
                        dissolver.ReplaceMaterials();

                    }
                    else
                    {
                        dissolver.RestoreDefaultMaterials();
                    }
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
        Debug.Log("Transition " + transitionTyp.ToString() + " active") ;
        if (transitionTyp == TransitionSelector.Dissolve)
        {
            ResetDissolve();
        }
        if (transitionTyp == TransitionSelector.Fade)
        {
            ResetFade();
        }
        if (transitionTyp == TransitionSelector.Translate)
        {
            ResetForTranslate();
        }
        if (transitionTyp == TransitionSelector.Combine)
        {
            ResetDissolve();
        }
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
    }
    IEnumerator RemoveTargetToReplica_Dissolve()
    {
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
    }
    IEnumerator RemoveTargetToReplica_Fade()
    {
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
    }
    IEnumerator AddTargetToTarget_Dissolve()
    {
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
        stopWatchEnd = Time.time - stopWatchStart;
        Debug.Log("Dissolve to Target: " + stopWatchEnd.ToString());
        stopWatchEnd = 0;
    }
    IEnumerator AddTargetToTarget_Combine()
    {
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
        StopCoroutine(AddTargetToTarget_Combine());
        stopWatchEnd = Time.time - stopWatchStart;
        Debug.Log(stopWatchEnd.ToString());
        stopWatchEnd = 0;
    }
    IEnumerator AddTargetToTarget_Fade()
    {
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
        stopWatchEnd = Time.time - stopWatchStart;
        Debug.Log("Fade to Target: " + stopWatchEnd.ToString());
        stopWatchEnd = 0;
    }
    IEnumerator RemoveReplicaToTarget_Dissolve()
    {
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
    }
    IEnumerator RemoveReplicaToTarget_Combine()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        Debug.Log("Start RemoveReplicaToTarget_Combine");
        foreach (serializableClass replica in replicaList)//int i = replicaList.Count - 1; i >= 0; i--
        {
            count++;
            foreach (GameObject replicaObject in replica.replicaObjects)//replicaList[i].replicaObjects
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                string name = dissolver.name;
                if (replicaObject.name == "Room")
                {
                    dissolver.Duration = durationRoom;
                }
                else
                {
                    dissolver.Duration = durationPerObject;
                }
                if (replicaTranslateStringList.Contains(name))
                {
                    dissolver.TranslateOut(durationPerObject);
                }
                else if (replicaFadeStringList.Contains(name))
                {
                    dissolver.FadeOut(durationPerObject);
                }
                else
                {
                    dissolver.Dissolve();
                }
            }
            if (count > 6)
            {
                Debug.Log("Call StartCoroutine(Target_1_ToReplica_I());");
                StartCoroutine(AddTargetToTarget_Combine());
                if (testWithVarjo) { Core.XRSceneManager.Instance.arVRToggle.SetModeToVR(); }
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveReplicaToTarget_Dissolve());
    }
    IEnumerator RemoveReplicaToTarget_Fade()
    {
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
    }
    IEnumerator RemoveReplicaToReal_Dissolve()
    {
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
        stopWatchEnd = Time.time - stopWatchStart;
        Debug.Log(stopWatchEnd.ToString());
        stopWatchEnd = 0;
    }
    IEnumerator RemoveReplicaToReal_Fade()
    {
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
        stopWatchEnd = Time.time - stopWatchStart ;
        Debug.Log(stopWatchEnd.ToString());
        stopWatchEnd = 0;
    }
    IEnumerator RemoveReplicaOnly()
    {
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
    }
    IEnumerator AddReplicaToReplicaAndTarget_Dissolve()
    {
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
        Debug.Log("BREAK!");
        yield return new WaitForSeconds(waitToFinischCoroutine);
        Debug.Log("BREAK END!");
        StartCoroutine(RemoveReplicaToTarget_Dissolve());
        //Debug.Log("Call StartCoroutine(RemoveReplicaToTarget())");

        foreach (GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(true);
        }
        //Debug.Log("End AddReplicaToReplicaAndTarget_Dissolve");
        StopCoroutine(AddReplicaToReplicaAndTarget_Dissolve());
    }
    IEnumerator AddReplicaToReplicaAndTarget_Combine()
    {
        int count = 0;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        foreach (serializableClass replica in replicaList)
        {
            count++;
            foreach (GameObject replicaObject in replica.replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                string name = dissolver.name;
                if(replicaTranslateStringList.Contains(name))
                {
                    dissolver.TranslateIn(durationPerObject);
                }
                else if (replicaFadeStringList.Contains(name))
                {
                    dissolver.FadeIn(durationPerObject);
                }
                else
                {
                    dissolver.Materialize();
                }
            }
            yield return wfs;

        }
        yield return new WaitForSeconds(waitToFinischCoroutine);
        StartCoroutine(RemoveReplicaToTarget_Combine());
        
        foreach (GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(true);
        }
        Debug.Log("End AddReplicaToReplicaAndTarget_Dissolve");
        StopCoroutine(AddReplicaToReplicaAndTarget_Combine());
    }
    IEnumerator AddReplicaToReplicaAndTarget_Fade()
    {
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
    }
    IEnumerator AddReplicaToReplica_Dissolve()
    {
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

    }
    IEnumerator AddReplicaToReplica_Fade()
    {
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

    }

    IEnumerator RemoveReplicaToReal_Translate()
    {


        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
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

                    if (replicaObject.name == "Room")
                    {
                        dissolver.TranslateOut(2f);
                    }
                    else
                    {
                        dissolver.TranslateOut(durationPerObject);
                    }
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


        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
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
                    if (replicaObject.name == "Room")
                    {
                        dissolver.TranslateOut(2f);
                    }
                    else
                    {
                        dissolver.TranslateOut(durationPerObject);
                    }
                }

            }
            if (count > 2 && !coroutineIsRunning_RemoveReplicaToTarget_Translate) 
            {
                count = 0;
                coroutineIsRunning_RemoveReplicaToTarget_Translate = true;
                StartCoroutine(AddTargetToTarget_Translate());
            }
            yield return wfs;
        }
    }
    IEnumerator RemoveReplicaOnly_Translate()
    {


        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);


        for (int i = replicaList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    //dissolver.startPosition = replicaObject.transform.position;
                    //dissolver.targetPosition = new Vector3(replicaObject.transform.position.x, replicaObject.transform.position.y+6f, replicaObject.transform.position.z);

                    dissolver.TranslateOut(durationPerObject);
                }

            }
            yield return wfs;
        }
        stopWatchEnd = Time.time - stopWatchStart;
        Debug.Log(stopWatchEnd.ToString());
        stopWatchEnd = 0;
    }
    IEnumerator RemoveTargetToReplica_Translate()
    {


        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);

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

                    dissolver.TranslateOut(durationPerObject);
                }

            }
            if (count > 1 && !coroutineIsRunning_AddReplicaToReplica_Translate)
            {
                count = 0;
                coroutineIsRunning_AddReplicaToReplica_Translate = true;
                StartCoroutine(AddReplicaToReplica_Translate());
            }
            yield return wfs;
        }

    }
    IEnumerator AddReplicaToReplica_Translate()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);


        for (int i = replicaList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateIn(durationPerObject);
                }

            }
            yield return wfs;
        }
        //Hier F2
        foreach(GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(false);
        }
        yield return new WaitForSeconds(waitToFinischCoroutine);
        StartCoroutine(RemoveReplicaOnly_Translate());
        coroutineIsRunning_AddReplicaToReplica_Translate = false;
    }

    IEnumerator AddReplicaToReplicaAndTarget_Translate()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);


        for (int i = 0; i < replicaList.Count; i++)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateIn(durationPerObject);
                }

            }
            yield return wfs;
        }
        //Hier F2
        yield return new WaitForSeconds(waitToFinischCoroutine);
        StartCoroutine(RemoveReplicaToTarget_Translate());
        foreach (GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(true);
        }
    }
    IEnumerator AddTargetToTarget_Translate()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);


        for (int i = 0; i < target_1_List.Count; i++)
        {
            foreach (GameObject replicaObject in target_1_List[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null)// && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateIn(durationPerObject);
                }

            }
            yield return wfs;
        }
        stopWatchEnd = Time.time - stopWatchStart;
        Debug.Log("Translate to Target: " + stopWatchEnd.ToString());

        stopWatchEnd = 0;
        coroutineIsRunning_RemoveReplicaToTarget_Translate = false;
    }

    private void ResetDissolve()
    {
        foreach (serializableClass replica in replicaList)
        {
            foreach (GameObject replicaObject in replica.replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null)
                {
                    dissolver.Duration = 0.01f;
                    dissolver.Dissolve();
                }

            }

        }
        foreach (serializableClass target in target_1_List)
        {
            foreach (GameObject targetObject in target.replicaObjects)
            {
                Dissolver dissolver = targetObject.GetComponent<Dissolver>();
                if (dissolver != null)
                {
                    dissolver.Duration = 0.01f;
                    dissolver.Dissolve();
                }
            }

        }
        foreach (GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(false);
        }
    }
    private void ResetFade()
    {
        foreach (serializableClass replica in replicaList)
        {
            foreach (GameObject replicaObject in replica.replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null)
                {
                    dissolver.Duration = 0.01f;
                    dissolver.FadeOut(0.01f);
                }
            }

        }
        foreach (serializableClass target in target_1_List)
        {
            foreach (GameObject targetObject in target.replicaObjects)
            {
                Dissolver dissolver = targetObject.GetComponent<Dissolver>();
                if (dissolver != null)
                {
                    dissolver.Duration = 0.01f;
                    dissolver.FadeOut(0.01f);
                }
            }

        }
        foreach (GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(false);
        }
    }
    private void ResetTranslate()
    {
        for (int i = 0; i < target_1_List.Count; i++)
        {
            foreach (GameObject targetObject in target_1_List[i].replicaObjects)
            {
                Dissolver dissolver = targetObject.GetComponent<Dissolver>();
                if (dissolver != null)// && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateIn(0.1f);
                }

            }
        }
        for (int i = 0; i < replicaList.Count; i++)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateIn(0.1f);
                }

            }
        }
        //ResetFade();
    }
    private void ResetForTranslate()
    {
        for (int i = 0; i < target_1_List.Count; i++)
        {
            foreach (GameObject targetObject in target_1_List[i].replicaObjects)
            {
                Dissolver dissolver = targetObject.GetComponent<Dissolver>();
                if (dissolver != null)// && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateOut(0.1f);
                }

            }
        }
        for (int i = 0; i < replicaList.Count; i++)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateOut(0.1f);
                }

            }
        }
        foreach (GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(false);
        }
    }
    private void ResetForCombine()
    {
        foreach (serializableClass replica in replicaList)
        {
            foreach (GameObject replicaObject in replica.replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null)
                {
                    dissolver.Duration = 0.01f;
                    if (dissolver.name == "rollo1" || dissolver.name == "rollo2")
                    {
                        dissolver.TranslateOut(durationPerObject);
                    }
                    else if (dissolver.name == "Roof" || dissolver.name == "Room")
                    {
                        dissolver.FadeOut(durationPerObject);
                    }
                    else
                    {
                        dissolver.Dissolve();
                    }
                    
                }

            }

        }
        foreach (serializableClass target in target_1_List)
        {
            foreach (GameObject targetObject in target.replicaObjects)
            {
                Dissolver dissolver = targetObject.GetComponent<Dissolver>();
                if (dissolver != null)
                {
                    dissolver.Duration = 0.01f;
                    dissolver.Dissolve();
                }
            }

        }
        foreach (GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(false);
        }
    }
}

