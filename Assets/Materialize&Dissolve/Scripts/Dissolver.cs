using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Dissolver : MonoBehaviour
{
    [System.Serializable]
    public class DissolveRenderer
    {
        public DissolveRenderer(Renderer renderer, List<Material> materials)
        {
            this.renderer = renderer;
            this.defaultMaterials = materials;
        }

        /// <summary>
        /// The renderer.
        /// </summary>
        public Renderer renderer;

        /// <summary>
        /// All materials found in the renderer when the class is created.
        /// </summary>
        public List<Material> defaultMaterials = new List<Material>();

        /// <summary>
        /// All materials that should replace "materials" list when ReplaceMaterials() function is called.
        /// </summary>
        public List<Material> materialsToReplace = new List<Material>();

        /// <summary>
        /// Should we replace the materials with the new ones when ReplaceMaterials() function is called.
        /// </summary>
        public bool shouldReplaceMaterials = false;
    }

    public enum MeshesDetection
    {
        GetComponents,GetComponentsInChildren,GetComponentsInParents
    }

    public enum DissolverState
    {
        Dissolved,Materialized
    }

    /// <summary>
    /// List of all mesh renderers with internal lists of current materials and materials to replace.
    /// </summary>
    public List<DissolveRenderer> DissolveRenderers;

    /// <summary>
    /// How long will it take to fully dissolve or materialize the object.
    /// </summary>
    public float Duration = 2.7f;

    /// <summary>
    /// When checked, automatically replaces renderers materials with the default ones.
    /// </summary>
    public bool AutomaticallyRestoreDefaultMaterials = false;

    /// <summary>
    /// Type of mesh detection used to find renderers with FindRenderers() function.
    /// </summary>
    public MeshesDetection meshesDetection;

    /// <summary>
    /// Initial state of the dissolver class.
    /// </summary>
    public DissolverState InitialState = DissolverState.Materialized;

    private bool m_Materialized = true;
    private bool m_Dissolved = false;

    private float m_DissolveAmount;
    private bool m_Finished = true;

    //Queue Coroutines

    public Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();


    void Start()
    {
        StartCoroutine(CoroutineCoordinator());
    }

    /// <summary>
    /// Set initial state for the dissolver script.
    /// </summary>
    /// <param name="state"> Choose </param>
    public void SetState(DissolverState state)
    {
        InitialState = state;

        switch (state)
        {
            case DissolverState.Dissolved:
                m_Materialized = false;
                m_Dissolved = true;
                break;
            case DissolverState.Materialized:
                m_Materialized = true;
                m_Dissolved = false;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Finds all renderers that would be affected by the dissolver. 
    /// </summary>
    public void FindRenderers()
    {
        List<Renderer> meshRenderers = new List<Renderer>();

        switch (meshesDetection)
        {
            case MeshesDetection.GetComponents:
                meshRenderers = new List<Renderer>(GetComponents<Renderer>());
                break;
            case MeshesDetection.GetComponentsInChildren:
                meshRenderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
                break;
            case MeshesDetection.GetComponentsInParents:
                meshRenderers = new List<Renderer>(GetComponentsInParent<Renderer>());
                break;
        }

        DissolveRenderers = new List<DissolveRenderer>();
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            DissolveRenderers.Add(new DissolveRenderer(meshRenderers[i],new List<Material>(meshRenderers[i].sharedMaterials)));
        }
    }

    /// <summary>
    /// Finds all renderers that would be affected by the dissolver. This function doesn't override internal meshesDetection state.
    /// </summary>
    /// <param name="meshesDetection">Type of mesh detection used to find renderers. </param>
    public void FindRenderers(MeshesDetection meshesDetection)
    {
        List<Renderer> meshRenderers = new List<Renderer>();

        switch (meshesDetection)
        {
            case MeshesDetection.GetComponents:
                meshRenderers = new List<Renderer>(GetComponents<Renderer>());
                break;
            case MeshesDetection.GetComponentsInChildren:
                meshRenderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
                break;
            case MeshesDetection.GetComponentsInParents:
                meshRenderers = new List<Renderer>(GetComponentsInParent<Renderer>());
                break;
        }

        DissolveRenderers = new List<DissolveRenderer>();
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            DissolveRenderers.Add(new DissolveRenderer(meshRenderers[i], new List<Material>(meshRenderers[i].sharedMaterials)));
        }

    }

    /// <summary>
    /// Replaces materials in renderers when shouldReplace variable is set to truth with materialsToReplace. Doesn't override defaultMaterials.
    /// </summary>
    public void ReplaceMaterials()
    {
        foreach (var obj in DissolveRenderers)
        {
            if (!obj.shouldReplaceMaterials) continue;

            if (obj.materialsToReplace == null)
            {
                Debug.LogWarning($"There is no materials to replace in {gameObject.name}");
                continue;
            }

            bool replace = true;

            var materials = new List<Material>();
            foreach (var mat in obj.materialsToReplace)
            {
                if (mat == null)
                {
                    Debug.LogWarning($"There are missing materials to replace in {gameObject.name}");
                    replace = false;
                    break;
                }
                materials.Add(mat);
            }
            if (replace)
            {
                obj.renderer.materials = materials.ToArray();

                // We are not everriding defaultMaterials so we could then come back to them when we are no longer using the effect.
                //obj.materials = materials;
            }
        }
    }

    /// <summary>
    /// Replaces materials of the all renderers with the default ones that were found on the renderers when FindRenderers() function was called.
    /// </summary>
    public void RestoreDefaultMaterials()
    {
        foreach (var obj in DissolveRenderers)
        {
            if (obj.defaultMaterials == null)
            {
                Debug.LogWarning($"There is no default materials to replace in {gameObject.name}");
                continue;
            }

            bool replace = true;

            var materials = new List<Material>();
            foreach (var mat in obj.defaultMaterials)
            {
                if (mat == null)
                {
                    Debug.LogWarning($"There are missing materials to replace in {gameObject.name}");
                    replace = false;
                    break;
                }
                materials.Add(mat);
            }
            if (replace)
            {
                obj.renderer.materials = materials.ToArray();

                // We are not everriding defaultMaterials so we could then come back to them when we are no longer using the effect.
                //obj.materials = materials;
            }
        }
    }

    /// <summary>
    /// Operation is executed if other operations were finished. Function automatically detects which operation to choose between materialize and dissolve.
    /// </summary>
    ///<returns>
    ///True if materialize or dissolve can be performed, otherwise false when the previous action has not ended.
    ///</returns>
    public bool MaterializeDissolve()
    {
        if (!m_Finished) return false;
        m_Finished = false;

        if (m_Dissolved)
            StartCoroutine(Materialize(Duration));
        else if (m_Materialized)
            StartCoroutine(Dissolve(Duration));

        return true;
    }

    /// <summary>
    /// Materialize operation independent of the automatic state detection of the dissolver.
    /// </summary>
    public void Materialize()
    {
        StartCoroutine(Materialize(Duration));
    }

    /// <summary>
    /// Dissolve operation independent of the automatic state detection of the dissolver.
    /// </summary>
    public void Dissolve()
    {
        StartCoroutine(Dissolve(Duration));
    }

    /// <summary>
    /// When called, operation is added to queue. Function automatically detects which operation to choose between materialize and dissolve.
    /// </summary>
    public void QueueMaterializeDissolve()
    {
        coroutineQueue.Enqueue(QueueMaterializeDissolve(Duration));
    }


    IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (coroutineQueue.Count > 0)
                yield return StartCoroutine(coroutineQueue.Dequeue());
            yield return null;
        }
    }

    private IEnumerator QueueMaterializeDissolve(float fadeDuration)
    {
        if(DissolveRenderers == null || DissolveRenderers.Count == 0)
        {
            FindRenderers();
        }

        float elapsedTime = 0f;

        if(m_Dissolved)
        {

            m_Materialized = true;
            m_Dissolved = false;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                m_DissolveAmount = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);

                foreach (var obj in DissolveRenderers)
                {
                    foreach (var mat in obj.renderer.materials)
                    {
                        mat.SetFloat("_DissolveAmount", m_DissolveAmount);
                    }
                }

                yield return null;
            }

            m_Finished = true;
        }

        else if(m_Materialized)
        {
            m_Materialized = false;
            m_Dissolved = true;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                m_DissolveAmount = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                foreach (var obj in DissolveRenderers)
                {
                    foreach (var mat in obj.renderer.materials)
                    {
                        mat.SetFloat("_DissolveAmount", m_DissolveAmount);
                    }
                }
                yield return null;
            }


            m_Finished = true;
        }

    }

    private IEnumerator Materialize(float fadeDuration)
    {
        if (DissolveRenderers == null || DissolveRenderers.Count == 0)
        {
            FindRenderers();
        }

        float elapsedTime = 0f;

        m_Materialized = true;
        m_Dissolved = false;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            m_DissolveAmount = Mathf.Lerp(1, 0,elapsedTime / fadeDuration);

            foreach (var obj in DissolveRenderers)
            {
                foreach (var mat in obj.renderer.materials)
                {
                    mat.SetFloat("_DissolveAmount", m_DissolveAmount);
                    Debug.Log("dissolve " + m_DissolveAmount + mat.name);
                }
            }
            yield return null;
        }

        m_Finished = true;

        if(AutomaticallyRestoreDefaultMaterials)
        {
            RestoreDefaultMaterials();
        }
    }

    private IEnumerator Dissolve(float fadeDuration)
    {
        if (DissolveRenderers == null || DissolveRenderers.Count == 0)
        {
            FindRenderers();
        }

        float elapsedTime = 0f;

        m_Materialized = false;
        m_Dissolved = true;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            m_DissolveAmount = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            foreach (var obj in DissolveRenderers)
            {
                foreach (var mat in obj.renderer.materials)
                {
                    mat.SetFloat("_DissolveAmount", m_DissolveAmount);
                }
            }
            yield return null;
        }
        m_Finished = true;

        if (AutomaticallyRestoreDefaultMaterials)
        {
            RestoreDefaultMaterials();
        }
    }
}
