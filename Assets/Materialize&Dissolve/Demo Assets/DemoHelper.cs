using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoHelper : MonoBehaviour
{
    private Dissolver dissolver;
    private bool wait = true;

    public Text m_text;
    public bool autom = true;

    void Start()
    {
        dissolver = GetComponent<Dissolver>();
    }

    void Update()
    {
        if(m_text) m_text.text = dissolver.coroutineQueue.Count.ToString();

        if (dissolver && wait)
        {
            if (!autom) return;
            if (dissolver.MaterializeDissolve())
            {
                //wait = false;
                //StartCoroutine(Coroutine());
            }

        }
    }

    IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(1.6f);
        dissolver.ReplaceMaterials();
        //wait = true;
    }
}
