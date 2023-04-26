using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ColliderCheck : MonoBehaviour
{
    public GameObject searchObj1;
    bool begin = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!begin && other.name == searchObj1.name)
        {
            Debug.Log("Search Target 1 found");
            StartCoroutine(StartPeriodicHaptics());
            begin = true;
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
}
