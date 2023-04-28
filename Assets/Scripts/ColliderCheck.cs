using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ColliderCheck : MonoBehaviour
{
    public List<GameObject> searchObjsT2 = new List<GameObject>();
    private List<string> searchObjsT2String = new List<string>();
    int searchNumber = 0;
    bool begin = false;
    bool isButtonClickable = true;
    public AudioSource audioSource;
    public AudioClip success;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject obj in searchObjsT2)
        {
            searchObjsT2String.Add(obj.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Keypad0) && isButtonClickable)
        {
            isButtonClickable = false;
            searchNumber = 0;
            Debug.Log("Search Number: " + searchNumber +" Search Obj: " + searchObjsT2String[searchNumber]);
            begin = false;
            StartCoroutine(EnableButtonAfterDebounce());
        }
        if (Input.GetKey(KeyCode.Keypad1) && isButtonClickable)
        {
            isButtonClickable = false;
            searchNumber = 1;
            Debug.Log("Search Number: " + searchNumber + " Search Obj: " + searchObjsT2String[searchNumber]);
            begin = false;
            StartCoroutine(EnableButtonAfterDebounce());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!begin && searchObjsT2String[searchNumber] == other.name)
        {
            Debug.Log(other.name + " found");
            StartCoroutine(StartPeriodicHaptics());
            begin = true;
            audioSource.PlayOneShot(success);
        }
        
    }
    IEnumerator StartPeriodicHaptics()
    {
        // Trigger haptics every second
        var delay = new WaitForSeconds(0.05f);
        int count = 20;
        int antiCount = 0;
        int ampliCounter;
        while (count > 0)
        {
            count--;
            SendHaptics(1f);
            yield return delay;
        }
    }

    void SendHaptics(float ampli)
    {
        ActionBasedController _rightControllerScript = this.GetComponent<ActionBasedController>();
        if (_rightControllerScript != null)
            _rightControllerScript.SendHapticImpulse(ampli, 1f);
    }

    private IEnumerator EnableButtonAfterDebounce()
    {
        // Wait for the debounce time
        yield return new WaitForSeconds(0.25f);

        // Set the button clickability flag to true
        isButtonClickable = true;
    }
}
