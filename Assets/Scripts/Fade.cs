using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public Fade fade;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FadeOut()
    {
        StartCoroutine(FadeOut_I(2.7f));

    }
    private IEnumerator FadeOut_I(float fadeDuration)
    {
        Renderer meshRenderer = GetComponent<Renderer>();
        Material[] mats = meshRenderer.materials;
        float alpha = 1.0f;
        float elapsedTime = 0f;


        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);

            foreach (Material mat in mats)
            {
                if (mat.name == "GlassMat (Instance)")
                {

                    Debug.Log(mat.name.ToString());
                }
                else
                {
                    Color cs = mat.color;
                    cs.a = alpha;
                    mat.color = cs;
                }

            }
            meshRenderer.materials = mats;
            yield return null;

        }
    }
}
