using Leap.Unity;
using Leap.Unity.Infix;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.InputSystem;
using static Transition;
using static UHI.Tracking.InteractionEngine.Examples.SimpleInteractionGlow;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.XR.Interaction.Toolkit;

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
    public List<GameObject> onlyTarget2Objs = new List<GameObject>();

    private List<string> replicaTranslateStringList = new List<string>();
    public List<GameObject> replicaTranslateList = new List<GameObject>();
    private List<string> replicaFadeStringList = new List<string>();
    public List<GameObject> replicaFadeList = new List<GameObject>();

    private List<string> target1TranslateStringList = new List<string>();
    public List<GameObject> target1TranslateList = new List<GameObject>();
    private List<string> target1FadeStringList = new List<string>();
    public List<GameObject> target1FadeList = new List<GameObject>();

    private List<string> target2TranslateStringList = new List<string>();
    public List<GameObject> target2TranslateList = new List<GameObject>();
    private List<string> target2FadeStringList = new List<string>();
    public List<GameObject> target2FadeList = new List<GameObject>();

    public List<GameObject> target1List = new List<GameObject>();
    public List<GameObject> target2List = new List<GameObject>();

    public List<serializableClass> target_2_List = new List<serializableClass>();

    private bool isVC = true;
    private bool isClouds = false;

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
    public Light lightNoNVC;
    public Light lightClouds;
    public Light AreaLight1;
    public Light AreaLight2;

    HDAdditionalLightData lightComponent; 

    public bool testWithVarjo = false;
    private float debounceTime = 0.25f;   // Debounce time in seconds
    private bool isButtonClickable = true;  // Flag to keep track of button clickability
    private float debounceTimeController = 15.25f;   // Debounce time in seconds
    private bool isControllerClickable = true;  // Flag to keep track of button clickability
    private float stopWatchStart = 0.0f;
    private float stopWatchEnd = 0.0f;

    private bool isTarget2 = false;
    private GameObject boxObj;

    public InputAction startTransitionAction;

    private bool isRealEnvironment = true;
    public GameObject leftController;
    public GameObject rightController;

    public AudioSource audioSource;
    public AudioClip transitionSound;

    public static Transition transitionScript;

    private void Awake()
    {
        transitionScript = this;
        startTransitionAction.started += ctx =>
        {
            Debug.Log("Init");
            if (isButtonClickable)
            {
                audioSource.PlayOneShot(transitionSound);

                StartCoroutine(StartPeriodicHaptics());

                if (isRealEnvironment)
                {
                    isRealEnvironment = ChangeToVR();
                }
                else
                {
                    isRealEnvironment = ChangeToReal();
                }
            }

        };

        startTransitionAction.performed += ctx =>
        {

            Debug.Log("action performed");

        };

        startTransitionAction.canceled += ctx =>
        { //if pressed too fast
            Debug.Log("action canceled");
        };
    }

    IEnumerator StartPeriodicHaptics()
    {
        // Trigger haptics every second
        var delay = new WaitForSeconds(0.1f);
        int count = 120;
        int antiCount = 0;
        int ampliCounter;
        while (count>0)
        {
            antiCount++;
            count--;
            ampliCounter = count;
            if (count < 60)
            {
                ampliCounter = antiCount;
            }
            SendHaptics(((ampliCounter / 120f)-1)*-1);
            yield return delay;
        }
    }

    public bool getIsTarget2()
    {
        return isTarget2;
    }
    void SendHaptics(float ampli)
    {
        ActionBasedController _rightControllerScript = rightController.GetComponent<ActionBasedController>();
        if (_rightControllerScript != null)
            _rightControllerScript.SendHapticImpulse(ampli, 1f);
    }

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
        if (true)
        {
            durationPerObject = 2.0f;           // Dissolve 11-12  Translate 11-12   Fade 11-12
            durationRoom = 3.5f;
            waitToFinischCoroutine = 2.5f;
            durationNextObj = 0.3f;
        }
        if (false)
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

        foreach (GameObject obj in target1TranslateList)
        {
            target1TranslateStringList.Add(obj.name);
        }
        foreach (GameObject obj in target1FadeList)
        {
            target1FadeStringList.Add(obj.name);
        }
        foreach (GameObject obj in target2TranslateList)
        {
            target2TranslateStringList.Add(obj.name);
        }
        foreach (GameObject obj in target2FadeList)
        {
            target2FadeStringList.Add(obj.name);
        }
        boxObj = GameObject.FindGameObjectWithTag("shadowBox");
        lightComponent = light.GetComponent<HDAdditionalLightData>();

    }


    private List<int> num = new List<int>();
    bool coroutineIsRunning_AddReplicaToReplica_Translate = false;
    bool coroutineIsRunning_RemoveReplicaToTarget_Translate = false;
    bool lastCallToReal = false;
    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKey(KeyCode.F1))
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
        }*/
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
        if (Input.GetKey(KeyCode.F5) && isControllerClickable) // Replica -> Target
        {
            if (isControllerClickable)
            {
                audioSource.PlayOneShot(transitionSound);

                StartCoroutine(StartPeriodicHaptics());

                if (isRealEnvironment)
                {
                    isRealEnvironment = ChangeToVR();
                }
                else
                {
                    isRealEnvironment = ChangeToReal();
                }
            }
        }
        if (Input.GetKey(KeyCode.F6) && isButtonClickable) //  Target -> Replica
        {

        }
        if (Input.GetKeyDown("v") && isButtonClickable) //  Target -> Replica
        {
            isButtonClickable = false;
            isVC = !isVC;
            if(isVC)
            {
                lightNoNVC.GetComponent<Light>().enabled = false;
                light.GetComponent<Light>().enabled = true;

                Debug.Log("VC is on");
            }
            else
            {
                light.GetComponent<Light>().enabled = false;
                lightNoNVC.GetComponent<Light>().enabled = true;

                Debug.Log("VC is off");
            }
            StartCoroutine(EnableButtonAfterDebounce());
        }
        if (Input.GetKeyDown("w") && isButtonClickable) //  Target -> Replica
        {
            isButtonClickable = false;
            isClouds = !isClouds;
            if (!isClouds)
            {
                lightClouds.GetComponent<Light>().enabled = false;
                light.GetComponent<Light>().enabled = true;
                AreaLight1.GetComponent<Light>().enabled = false;
                AreaLight2.GetComponent<Light>().enabled = false;
                Debug.Log("Sun Clouds is off");
            }
            else
            {
                light.GetComponent<Light>().enabled = false;
                lightClouds.GetComponent<Light>().enabled = true;
                AreaLight1.GetComponent<Light>().enabled = true;
                AreaLight2.GetComponent<Light>().enabled = true;
                Debug.Log("Sun Clouds is on");
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
        if (Input.GetKeyDown("s"))
        {
            lightComponent.RequestShadowMapRendering();
        }
        if (Input.GetKey(KeyCode.F1) && isButtonClickable) //  Target -> Replica
        {
            isButtonClickable = false;
            isTarget2 = false;
            foreach(GameObject obj in target1List)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in target2List)
            {
                obj.SetActive(false);
            }
            StartCoroutine(EnableButtonAfterDebounce());
        }
        if (Input.GetKey(KeyCode.F2) && isButtonClickable) //  Target -> Replica
        {
            isButtonClickable = false;
            isTarget2 = true;
            foreach (GameObject obj in target1List)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in target2List)
            {
                obj.SetActive(true);
            }
            StartCoroutine(EnableButtonAfterDebounce());
        }
    }

    private bool ChangeToVR()
    {
        stopWatchStart = Time.time;
        isControllerClickable = false;
        if (isTarget2)
        {
            switch (currentTransition)
            {
                case TransitionSelector.Fade:
                    StartCoroutine(AddReplicaToReplicaAndTarget2_Fade());
                    break;

                case TransitionSelector.Dissolve:
                    StartCoroutine(AddReplicaToReplicaAndTarget2_Dissolve());
                    break;

                case TransitionSelector.Translate:
                    StartCoroutine(AddReplicaToReplicaAndTarget2_Translate());
                    break;
                case TransitionSelector.Combine:
                    StartCoroutine(AddReplicaToReplicaAndTarget2_Combine());
                    break;
                default:
                    break;
            }
        }
        else
        {
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
        }

        StartCoroutine(EnableControllerAfterDebounce());
        return false;
    }
    private bool ChangeToReal()
    {
        stopWatchStart = Time.time;
        isControllerClickable = false;
        if (isTarget2)
        {
            switch (currentTransition)
            {
                case TransitionSelector.Fade:
                    StartCoroutine(RemoveTarget2ToReplica_Fade());
                    break;

                case TransitionSelector.Dissolve:
                    StartCoroutine(RemoveTarget2ToReplica_Dissolve());
                    break;

                case TransitionSelector.Translate:
                    StartCoroutine(RemoveTarget2ToReplica_Translate());
                    break;
                case TransitionSelector.Combine:
                    StartCoroutine(RemoveTarget2ToReplica_Combine());
                    break;
                default:
                    break;
            }
        }
        else
        {
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
                case TransitionSelector.Combine:
                    StartCoroutine(RemoveTargetToReplica_Combine());
                    break;
                default:
                    break;
            }
        }
        StartCoroutine(EnableControllerAfterDebounce());
        return true;
    }
    private void OnEnable()
    {
        startTransitionAction.Enable();
    }

    private void OnDisable()
    {
        startTransitionAction.Disable();
    }

    private IEnumerator EnableButtonAfterDebounce()
    {
        // Wait for the debounce time
        yield return new WaitForSeconds(debounceTime);

        // Set the button clickability flag to true
        isButtonClickable = true;
    }
    private IEnumerator EnableControllerAfterDebounce()
    {
        // Wait for the debounce time
        yield return new WaitForSeconds(debounceTimeController);

        // Set the button clickability flag to true
        isControllerClickable = true;
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
                    /*if(dissolver.name == "rollo1" || dissolver.name == "Jalusie" || dissolver.name == "rollo2" || dissolver.name == "Curtain1" || dissolver.name == "Curtain2" || dissolver.name == "Curtain3")
                    {
                        dissolver.ReplaceMaterials();
                    }
                    else if (dissolver.name == "Room" || dissolver.name == "Roof" || dissolver.name == "Door" || dissolver.name == "Handle" || dissolver.name == "Lock") {
                        dissolver.ReplaceMaterials();

                    }*/
                    if (replicaFadeStringList.Contains(dissolver.name))
                    {
                        dissolver.ReplaceMaterials();
                    }
                    else if (replicaTranslateStringList.Contains(dissolver.name))
                    {
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
                if (transitionTyp == TransitionSelector.Combine)
                {
                    if (target1FadeStringList.Contains(dissolver.name))
                    {
                        dissolver.ReplaceMaterials();
                    }
                    else if (target1TranslateStringList.Contains(dissolver.name))
                    {
                        dissolver.ReplaceMaterials();
                    }
                    else
                    {
                        dissolver.RestoreDefaultMaterials();
                    }
                }
            }
        }

        foreach (serializableClass target in target_2_List)
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
                    Debug.Log(dissolver.name);

                    dissolver.RestoreDefaultMaterials();
                }
                if (transitionTyp == TransitionSelector.Translate)
                {
                    dissolver.ReplaceMaterials();
                }
                if (transitionTyp == TransitionSelector.Combine)
                {
                    if (target2FadeStringList.Contains(dissolver.name))
                    {
                        dissolver.ReplaceMaterials();
                    }
                    else if (target2TranslateStringList.Contains(dissolver.name))
                    {
                        dissolver.ReplaceMaterials();
                    }
                    else
                    {
                        dissolver.RestoreDefaultMaterials();
                    }
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
                LightToReplica();
                StartCoroutine(AddReplicaToReplica_Dissolve());
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveTargetToReplica_Dissolve());
    }
    IEnumerator RemoveTarget2ToReplica_Dissolve()
    {
        int count = 0;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        foreach (serializableClass target in target_2_List)
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
            if (count > 4)
            {
                LightToReplica();
                StartCoroutine(AddReplicaToReplica_2_Dissolve());
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveTarget2ToReplica_Dissolve());
    }
    IEnumerator RemoveTargetToReplica_Combine()
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
                string name = dissolver.name;
                if (target1TranslateStringList.Contains(name))
                {
                    dissolver.TranslateOut(durationPerObject + 1f);
                }
                else if (target1FadeStringList.Contains(name))
                {
                    dissolver.FadeOut(durationPerObject);
                }
                else
                {
                    dissolver.Dissolve();
                }
            }
            Debug.Log(count);
            if (count > 6)
            {
                LightToReplica();
                StartCoroutine(AddReplicaToReplica_Combine());
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveTargetToReplica_Dissolve());
    }
    IEnumerator RemoveTarget2ToReplica_Combine()
    {
        int count = 0;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        for (int i = target_2_List.Count - 1; i >= 0; i--)
        {
            count++;
            foreach (GameObject replicaObject in target_2_List[i].replicaObjects)
            {
                //Debug.Log(replicaObject.name);
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                string name = dissolver.name;
                if (target2TranslateStringList.Contains(name))
                {
                    dissolver.TranslateOut(durationPerObject + 1f);
                }
                else if (target2FadeStringList.Contains(name))
                {
                    dissolver.FadeOut(durationPerObject);
                }
                else
                {
                    dissolver.Dissolve();
                }
            }
            Debug.Log(count);
            if (count > 3)
            {
                LightToReplica();
                StartCoroutine(AddReplicaToReplica2_Combine());
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
                LightToReplica();
                StartCoroutine(AddReplicaToReplica_Fade());
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveTargetToReplica_Dissolve());
    }
    IEnumerator RemoveTarget2ToReplica_Fade()
    {
        int count = 0;
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        foreach (serializableClass target in target_2_List)
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
            if (count > 4)
            {
                LightToReplica();
                StartCoroutine(AddReplicaToReplica2_Fade());
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveTarget2ToReplica_Fade());
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
        lightComponent.RequestShadowMapRendering();
    }
    IEnumerator AddTargetToTarget2_Dissolve()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        for (int i = 0; i < target_2_List.Count; i++)
        {
            foreach (GameObject replicaObject in target_2_List[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                Debug.Log(dissolver.name);
                dissolver.Duration = durationPerObject;
                dissolver.Materialize();
            }
            yield return wfs;
        }
        lightComponent.RequestShadowMapRendering();

        Debug.Log("End");
        StopCoroutine(AddReplicaToReplicaAndTarget2_Dissolve());
        stopWatchEnd = Time.time - stopWatchStart;
        Debug.Log("Dissolve to Target 2: " + stopWatchEnd.ToString());
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

                string name = dissolver.name;
                Debug.Log(name);
                if (target1TranslateStringList.Contains(name))
                {
                    dissolver.TranslateIn(durationPerObject + 1f);
                }
                else if (target1FadeStringList.Contains(name))
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

        Debug.Log("End");
        StopCoroutine(AddTargetToTarget_Combine());
        stopWatchEnd = Time.time - stopWatchStart;
        Debug.Log(stopWatchEnd.ToString());
        stopWatchEnd = 0;
    }
    IEnumerator AddTargetToTarget2_Combine()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        for (int i = target_2_List.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in target_2_List[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;

                string name = dissolver.name;
                Debug.Log(name);
                if (target2TranslateStringList.Contains(name))
                {
                    dissolver.TranslateIn(durationPerObject + 1f);
                }
                else if (target2FadeStringList.Contains(name))
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

        Debug.Log("End");
        StopCoroutine(AddTargetToTarget2_Combine());
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
    IEnumerator AddTargetToTarget2_Fade()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        for (int i = target_2_List.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in target_2_List[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.FadeIn(durationPerObject);
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(AddTargetToTarget2_Fade());
        stopWatchEnd = Time.time - stopWatchStart;
        Debug.Log("Fade to Target 2: " + stopWatchEnd.ToString());
        stopWatchEnd = 0;
        lightComponent.RequestShadowMapRendering();

    }
    private void LightToTarget1()
    {
        light.GetComponent<Light>().enabled = false;
        lightClouds.GetComponent<Light>().enabled = false;
        lightNoNVC.GetComponent<Light>().enabled = true;
        //lightNoNVC.GetComponent<HDAdditionalLightData>().SetIntensity(5000);
        AreaLight1.GetComponent<Light>().enabled = false;
        AreaLight2.GetComponent<Light>().enabled = false;
    }
    private void LightToTarget2()
    {
        light.GetComponent<Light>().enabled = false;
        lightClouds.GetComponent<Light>().enabled = false;
        lightNoNVC.GetComponent<Light>().enabled = true;
        //lightNoNVC.GetComponent<HDAdditionalLightData>().SetIntensity(6500);
        AreaLight1.GetComponent<Light>().enabled = false;
        AreaLight2.GetComponent<Light>().enabled = false;
    }
    private void LightToReplica()
    {
        if (isClouds)
        {
            light.GetComponent<Light>().enabled = false;
            lightNoNVC.GetComponent<Light>().enabled = false;
            lightClouds.GetComponent<Light>().enabled = true;
            AreaLight1.GetComponent<Light>().enabled = true;
            AreaLight2.GetComponent<Light>().enabled = true;
        }
        else if (isVC)
        {
            lightClouds.GetComponent<Light>().enabled = false;
            lightNoNVC.GetComponent<Light>().enabled = false;
            light.GetComponent<Light>().enabled = true;
            AreaLight1.GetComponent<Light>().enabled = false;
            AreaLight2.GetComponent<Light>().enabled = false;
        }
        else // non VC Condition
        {
            lightClouds.GetComponent<Light>().enabled = false;
            light.GetComponent<Light>().enabled = false;
            lightNoNVC.GetComponent<Light>().enabled = true;
            AreaLight1.GetComponent<Light>().enabled = false;
            AreaLight2.GetComponent<Light>().enabled = false;
            //lightNoNVC.GetComponent<HDAdditionalLightData>().SetIntensity(13000);

        }
    }
    IEnumerator RemoveReplicaToTarget_Dissolve()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        Debug.Log("Start RemoveReplicaToTarget");
        List<serializableClass> tempList = replicaList;
        tempList.Swap(6, 11);
        foreach (serializableClass replica in tempList)//int i = replicaList.Count - 1; i >= 0; i--
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
                boxObj.SetActive(false);
                LightToTarget1();
                lightComponent.RequestShadowMapRendering();
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveReplicaToTarget_Dissolve());
    }
    IEnumerator RemoveReplicaToTarget2_Dissolve()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        Debug.Log("Start RemoveReplicaToTarget");
        List<serializableClass> tempList = replicaList;
        tempList.Swap(6, 11);
        foreach (serializableClass replica in tempList)//int i = replicaList.Count - 1; i >= 0; i--
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
            if (count > 6)
            {
                Debug.Log("Call StartCoroutine(Target_2_ToReplica_I());");
                StartCoroutine(AddTargetToTarget2_Dissolve());
                if (testWithVarjo) { Core.XRSceneManager.Instance.arVRToggle.SetModeToVR(); }
                boxObj.SetActive(false);

                LightToTarget2();

                lightComponent.RequestShadowMapRendering();
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveReplicaToTarget2_Dissolve());
    }
    IEnumerator RemoveReplicaToTarget_Combine()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        Debug.Log("Start RemoveReplicaToTarget_Combine");
        List<serializableClass> tempList = replicaList;
        tempList.Swap(6, 11);
        foreach (serializableClass replica in tempList)//int i = replicaList.Count - 1; i >= 0; i--
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
                if (replicaTranslateStringList.Contains(name) && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateOut(durationPerObject+1f);
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
                boxObj.SetActive(false);
                LightToTarget1();
                lightComponent.RequestShadowMapRendering();
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveReplicaToTarget_Dissolve());
    }
    IEnumerator RemoveReplicaToTarget2_Combine()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        Debug.Log("Start RemoveReplicaToTarget2_Combine");
        List<serializableClass> tempList = replicaList;
        tempList.Swap(6, 11);
        foreach (serializableClass replica in tempList)//int i = replicaList.Count - 1; i >= 0; i--
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
                if (replicaTranslateStringList.Contains(name) && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateOut(durationPerObject + 1f);
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
                Debug.Log("Call StartCoroutine(Target_2_ToReplica_I());");
                StartCoroutine(AddTargetToTarget2_Combine());
                if (testWithVarjo) { Core.XRSceneManager.Instance.arVRToggle.SetModeToVR(); }
                count = 0;
                boxObj.SetActive(false);
                LightToTarget2();

                lightComponent.RequestShadowMapRendering();
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
        List<serializableClass> tempList = replicaList;
        tempList.Swap(6, 11);
        foreach (serializableClass replica in tempList)//int i = replicaList.Count - 1; i >= 0; i--
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
                boxObj.SetActive(false);
                LightToTarget1();
                lightComponent.RequestShadowMapRendering();
                count = 0;
            }
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveReplicaToTarget_Fade());
    }
    IEnumerator RemoveReplicaToTarget2_Fade()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        Debug.Log("Start RemoveReplicaToTarget_Fade");
        List<serializableClass> tempList = replicaList;
        tempList.Swap(6, 11);
        foreach (serializableClass replica in tempList)//int i = replicaList.Count - 1; i >= 0; i--
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
                Debug.Log("Call StartCoroutine(AddTargetToTarget2_Fade);");
                StartCoroutine(AddTargetToTarget2_Fade());
                if (testWithVarjo) { Core.XRSceneManager.Instance.arVRToggle.SetModeToVR(); }
                boxObj.SetActive(false);
                LightToTarget2();
                lightComponent.RequestShadowMapRendering();
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
        List<serializableClass> tempList = replicaList;
        tempList.Swap(6, 11);
        for (int i = tempList.Count - 1; i >= 0; i--)//int i = replicaList.Count - 1; i >= 0; i--
        {
            count++;
            foreach (GameObject replicaObject in tempList[i].replicaObjects)//replicaList[i].replicaObjects
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
    IEnumerator RemoveReplicaToReal_Combine()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        for (int i = replicaList.Count - 1; i >= 0; i--)//int i = replicaList.Count - 1; i >= 0; i--
        {
            count++;
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)//replicaList[i].replicaObjects
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                string name = dissolver.name;
                if (replicaObject.name == "Room")
                {
                    dissolver.Duration = durationRoom;//
                }
                else
                {
                    dissolver.Duration = durationPerObject;
                }
                if (replicaTranslateStringList.Contains(name) && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateOut(durationPerObject + 1f);
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
            yield return wfs;
        }

        Debug.Log("End");
        StopCoroutine(RemoveReplicaToReal_Combine());
        stopWatchEnd = Time.time - stopWatchStart;
        Debug.Log(stopWatchEnd.ToString());
        stopWatchEnd = 0;
    }
    IEnumerator RemoveReplicaToReal_Fade()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        List<serializableClass> tempList = replicaList;
        tempList.Swap(6, 11);
        for (int i = tempList.Count - 1; i >= 0; i--)//int i = replicaList.Count - 1; i >= 0; i--
        {
            count++;
            foreach (GameObject replicaObject in tempList[i].replicaObjects)//replicaList[i].replicaObjects
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
    IEnumerator AddReplicaToReplicaAndTarget2_Dissolve()
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
        StartCoroutine(RemoveReplicaToTarget2_Dissolve());
        //Debug.Log("Call StartCoroutine(RemoveReplicaToTarget())");

        foreach (GameObject obj in onlyTarget2Objs)
        {
            obj.SetActive(true);
        }
        //Debug.Log("End AddReplicaToReplicaAndTarget_Dissolve");
        StopCoroutine(AddReplicaToReplicaAndTarget2_Dissolve());
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
                if(replicaTranslateStringList.Contains(name) && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateIn(durationPerObject+1f);
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
    IEnumerator AddReplicaToReplicaAndTarget2_Combine()
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
                if (replicaTranslateStringList.Contains(name) && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateIn(durationPerObject + 1f);
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
        StartCoroutine(RemoveReplicaToTarget2_Combine());

        foreach (GameObject obj in onlyTarget2Objs)
        {
            obj.SetActive(true);
        }
        Debug.Log("End AddReplicaToReplicaAndTarget2_Combine");
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
    IEnumerator AddReplicaToReplicaAndTarget2_Fade()
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
        StartCoroutine(RemoveReplicaToTarget2_Fade());
        Debug.Log("Call RemoveReplicaToTarget2_Fade");

        foreach (GameObject obj in onlyTarget2Objs)
        {
            obj.SetActive(true);
        }
        Debug.Log("End AddReplicaToReplicaAndTarget2_Fade");
        StopCoroutine(AddReplicaToReplicaAndTarget2_Fade());
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

        boxObj.SetActive(true);
        lightComponent.RequestShadowMapRendering();

        yield return new WaitForSeconds(waitToFinischCoroutine);
        StartCoroutine(RemoveReplicaToReal_Dissolve()); //hier ende
        
        StopCoroutine(AddReplicaToReplica_Dissolve());

    }
    IEnumerator AddReplicaToReplica_2_Dissolve()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        for (int i = replicaList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                dissolver.Materialize();
            }
            yield return wfs;
        }
        foreach (GameObject obj in onlyTarget2Objs)
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
    IEnumerator AddReplicaToReplica_Combine()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        for (int i = replicaList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                string name = dissolver.name;
                if (replicaTranslateStringList.Contains(name) && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateIn(durationPerObject + 1f);
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
        foreach (GameObject obj in onlyTarget1Objs)
        {
            obj.SetActive(false);
        }
        Debug.Log("End");
        if (testWithVarjo)
        {
            Core.XRSceneManager.Instance.arVRToggle.SetModeToAR();
        }
        boxObj.SetActive(true);
        lightComponent.RequestShadowMapRendering();
        yield return new WaitForSeconds(waitToFinischCoroutine);
        StartCoroutine(RemoveReplicaToReal_Combine()); //hier ende

        StopCoroutine(AddReplicaToReplica_Combine());

    }
    IEnumerator AddReplicaToReplica2_Combine()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        for (int i = replicaList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                dissolver.Duration = durationPerObject;
                string name = dissolver.name;
                if (replicaTranslateStringList.Contains(name) && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateIn(durationPerObject + 1f);
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
        foreach (GameObject obj in onlyTarget2Objs)
        {
            obj.SetActive(false);
        }
        Debug.Log("End");
        if (testWithVarjo)
        {
            Core.XRSceneManager.Instance.arVRToggle.SetModeToAR();
        }
        boxObj.SetActive(true);
        lightComponent.RequestShadowMapRendering();
        yield return new WaitForSeconds(waitToFinischCoroutine);
        StartCoroutine(RemoveReplicaToReal_Combine()); //hier ende

        StopCoroutine(AddReplicaToReplica_Combine());

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
        boxObj.SetActive(true);
        lightComponent.RequestShadowMapRendering();
        yield return new WaitForSeconds(waitToFinischCoroutine);
        StartCoroutine(RemoveReplicaToReal_Fade()); //hier ende

        StopCoroutine(AddReplicaToReplica_Fade());

    }
    IEnumerator AddReplicaToReplica2_Fade()
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
        foreach (GameObject obj in onlyTarget2Objs)
        {
            obj.SetActive(false);
        }
        Debug.Log("End");
        if (testWithVarjo)
        {
            Core.XRSceneManager.Instance.arVRToggle.SetModeToAR();
        }
        boxObj.SetActive(true);
        lightComponent.RequestShadowMapRendering();
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
                if (testWithVarjo) { Core.XRSceneManager.Instance.arVRToggle.SetModeToVR(); }
            }
            yield return wfs;
        }
    }
    IEnumerator RemoveReplicaToTarget_Translate()
    {


        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        List<serializableClass> tempList = replicaList;
        tempList.Swap(6, 11);
        foreach (serializableClass replica in tempList)
        {
            count++;
            foreach (GameObject replicaObject in replica.replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2" || dissolver.transform.parent.name == "NEC"))
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
            if (count > 4 && !coroutineIsRunning_RemoveReplicaToTarget_Translate) 
            {
                count = 0;
                coroutineIsRunning_RemoveReplicaToTarget_Translate = true;
                if (testWithVarjo) { Core.XRSceneManager.Instance.arVRToggle.SetModeToVR(); }
                StartCoroutine(AddTargetToTarget_Translate());
                boxObj.SetActive(false);
                LightToTarget1();
                lightComponent.RequestShadowMapRendering();
            }
            yield return wfs;
        }
    }
    IEnumerator RemoveReplicaToTarget2_Translate()
    {


        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);
        int count = 0;
        List<serializableClass> tempList = replicaList;
        tempList.Swap(6, 11);
        foreach (serializableClass replica in tempList)
        {
            count++;
            foreach (GameObject replicaObject in replica.replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2" || dissolver.transform.parent.name == "NEC"))
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
            if (count > 4 && !coroutineIsRunning_RemoveReplicaToTarget_Translate)
            {
                count = 0;
                coroutineIsRunning_RemoveReplicaToTarget_Translate = true;
                if (testWithVarjo) { Core.XRSceneManager.Instance.arVRToggle.SetModeToVR(); }
                StartCoroutine(AddTargetToTarget2_Translate());
                boxObj.SetActive(false);
                LightToTarget2();

                lightComponent.RequestShadowMapRendering();
            }
            yield return wfs;
        }
    }
    IEnumerator RemoveReplicaOnly_Translate()
    {


        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);

        List<serializableClass> tempList = replicaList;
        tempList.Swap(6, 11);
        for (int i = tempList.Count - 1; i >= 0; i--)
        {
            foreach (GameObject replicaObject in tempList[i].replicaObjects)
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
        for (int i = 0; i < target_1_List.Count; i++)//int i = target_1_List.Count - 1; i >= 0; i--
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
                LightToReplica();
                coroutineIsRunning_AddReplicaToReplica_Translate = true;
                StartCoroutine(AddReplicaToReplica_Translate());
            }
            yield return wfs;
        }

    }
    IEnumerator RemoveTarget2ToReplica_Translate()
    {


        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);

        int count = 0;
        for (int i = target_2_List.Count - 1; i >= 0; i--)//int i = target_1_List.Count - 1; i >= 0; i--
        {
            count++;
            foreach (GameObject replicaObject in target_2_List[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null)// && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    //dissolver.startPosition = replicaObject.transform.position;
                    //dissolver.targetPosition = new Vector3(replicaObject.transform.position.x, replicaObject.transform.position.y+6f, replicaObject.transform.position.z);

                    dissolver.TranslateOut(durationPerObject+1f);
                }

            }
            if (count > 2 && !coroutineIsRunning_AddReplicaToReplica_Translate)
            {
                count = 0;
                LightToReplica();
                coroutineIsRunning_AddReplicaToReplica_Translate = true;
                StartCoroutine(AddReplicaToReplica2_Translate());
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
        if (testWithVarjo)
        {
            Core.XRSceneManager.Instance.arVRToggle.SetModeToAR();
        }
        boxObj.SetActive(true);
        lightComponent.RequestShadowMapRendering();
        yield return new WaitForSeconds(waitToFinischCoroutine);
        StartCoroutine(RemoveReplicaOnly_Translate());
        coroutineIsRunning_AddReplicaToReplica_Translate = false;
    }
    IEnumerator AddReplicaToReplica2_Translate()
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
        foreach (GameObject obj in onlyTarget2Objs)
        {
            obj.SetActive(false);
        }
        if (testWithVarjo)
        {
            Core.XRSceneManager.Instance.arVRToggle.SetModeToAR();
        }
        boxObj.SetActive(true);
        lightComponent.RequestShadowMapRendering();
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
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2" || dissolver.transform.parent.name == "NEC"))
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
    IEnumerator AddReplicaToReplicaAndTarget2_Translate()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);


        for (int i = 0; i < replicaList.Count; i++)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2" || dissolver.transform.parent.name == "NEC"))
                {
                    dissolver.TranslateIn(durationPerObject);
                }

            }
            yield return wfs;
        }
        //Hier F2
        yield return new WaitForSeconds(waitToFinischCoroutine);
        StartCoroutine(RemoveReplicaToTarget2_Translate());
        foreach (GameObject obj in onlyTarget2Objs)
        {
            obj.SetActive(true);
        }
    }
    IEnumerator AddTargetToTarget_Translate()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);


        for (int i = target_1_List.Count - 1; i >= 0; i--)
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
    IEnumerator AddTargetToTarget2_Translate()
    {
        WaitForSeconds wfs = new WaitForSeconds(durationNextObj);


        for (int i = 0; i < target_2_List.Count; i++)
        {
            foreach (GameObject replicaObject in target_2_List[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null)// && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                {
                    dissolver.TranslateIn(durationPerObject+1f);
                }

            }
            yield return wfs;
        }
        stopWatchEnd = Time.time - stopWatchStart;
        Debug.Log("Translate to Target 2: " + stopWatchEnd.ToString());

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
        if (isTarget2)
        {
            foreach (serializableClass target in target_2_List)
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
            foreach (GameObject obj in onlyTarget2Objs)
            {
                obj.SetActive(false);
            }

        }
        else
        {
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
        if (isTarget2)
        {
            foreach (serializableClass target in target_2_List)
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
            foreach (GameObject obj in onlyTarget2Objs)
            {
                obj.SetActive(false);
            }

        }
        else
        {
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


    }
    private void ResetTranslate()
    {
        if (isTarget2)
        {
            for (int i = 0; i < target_2_List.Count; i++)
            {
                foreach (GameObject targetObject in target_2_List[i].replicaObjects)
                {
                    Dissolver dissolver = targetObject.GetComponent<Dissolver>();
                    if (dissolver != null)// && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                    {
                        dissolver.TranslateIn(0.1f);
                    }

                }
            }
        }
        else
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
        }

        for (int i = 0; i < replicaList.Count; i++)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2" || dissolver.transform.parent.name == "NEC"))
                {
                    dissolver.TranslateIn(0.1f);
                }

            }
        }
        //ResetFade();
    }
    private void ResetForTranslate()
    {
        if (isTarget2)
        {
            for (int i = 0; i < target_2_List.Count; i++)
            {
                foreach (GameObject targetObject in target_2_List[i].replicaObjects)
                {
                    Dissolver dissolver = targetObject.GetComponent<Dissolver>();
                    if (dissolver != null)// && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2"))
                    {
                        dissolver.TranslateOut(0.1f);
                    }

                }
            }
            foreach (GameObject obj in onlyTarget2Objs)
            {
                obj.SetActive(false);
            }
        }
        else
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
            foreach (GameObject obj in onlyTarget1Objs)
            {
                obj.SetActive(false);
            }
        }

        for (int i = 0; i < replicaList.Count; i++)
        {
            foreach (GameObject replicaObject in replicaList[i].replicaObjects)
            {
                Dissolver dissolver = replicaObject.GetComponent<Dissolver>();
                if (dissolver != null && (dissolver.transform.parent.name == "RoomOutline" || dissolver.transform.parent.name == "display_1" || dissolver.transform.parent.name == "display_2" || dissolver.transform.parent.name == "NEC"))
                {
                    dissolver.TranslateOut(0.1f);
                }

            }
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
                    if (dissolver.name == "rollo1" || dissolver.name == "rollo2" || dissolver.name == "Curtain1" || dissolver.name == "Curtain2" || dissolver.name == "Curtain3")
                    {
                        dissolver.TranslateOut(durationPerObject);
                    }
                    else if (dissolver.name == "Roof" || dissolver.name == "Room" || dissolver.name == "Door" || dissolver.name == "Handle" || dissolver.name == "Lock")
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
        if (isTarget2)
        {
            foreach (serializableClass target in target_2_List)
            {
                foreach (GameObject targetObject in target.replicaObjects)
                {
                    Dissolver dissolver = targetObject.GetComponent<Dissolver>();
                    if (dissolver != null)
                    {
                        dissolver.Duration = 0.01f;
                        if (target2FadeStringList.Contains(dissolver.name))
                        {
                            dissolver.FadeOut(durationPerObject);
                        }
                        else if (target2TranslateStringList.Contains(dissolver.name))
                        {
                            dissolver.TranslateOut(durationPerObject);
                        }
                        else
                        {
                            dissolver.Dissolve();
                        }
                    }
                }

            }
            foreach (GameObject obj in onlyTarget2Objs)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            foreach (serializableClass target in target_1_List)
            {
                foreach (GameObject targetObject in target.replicaObjects)
                {
                    Dissolver dissolver = targetObject.GetComponent<Dissolver>();
                    if (dissolver != null)
                    {
                        dissolver.Duration = 0.01f;
                        if (target1FadeStringList.Contains(dissolver.name))
                        {
                            dissolver.FadeOut(durationPerObject);
                        }
                        else if (target1TranslateStringList.Contains(dissolver.name))
                        {
                            dissolver.TranslateOut(durationPerObject);
                        }
                        else
                        {
                            dissolver.Dissolve();
                        }
                    }
                }

            }
            foreach (GameObject obj in onlyTarget1Objs)
            {
                obj.SetActive(false);
            }
        }


    }
}

