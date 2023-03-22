using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveAPITest : MonoBehaviour
{
    private Dissolver dissolver;

    // Dissolve material used to materialize and dissolve the object
    public Material dissolveMaterial;

    void Start()
    {
        // We are adding Dissolver script from the code
        dissolver = gameObject.AddComponent<Dissolver>();

        // Setting duration of the action.
        dissolver.Duration = 5f;

        // Makes sure that after action has ended, the default materials are restored.
        dissolver.AutomaticallyRestoreDefaultMaterials = true;

        // You can get renderers of the object like that:
        dissolver.meshesDetection = Dissolver.MeshesDetection.GetComponentsInChildren;
        dissolver.FindRenderers();

        // Or that:
        dissolver.FindRenderers(Dissolver.MeshesDetection.GetComponentsInChildren);

        // We are iterating over all dissolve renderers
        foreach (var item in dissolver.DissolveRenderers)
        {
            // In order to replace the unity renderers materials we need to set shouldReplaceMaterials to true.
            item.shouldReplaceMaterials = true;

            // And add the dissolve material to the list of materials to replace
            item.materialsToReplace.Add(dissolveMaterial);

            // NOTE: You need to make sure that the number of the materialsToReplace is matching the number of materials in you renderer,
            // i.e. if your renderer has 3 materials you need to add 3 materials to replace them. 
        }

        // Replacing materials with the dissolveMaterial
        dissolver.ReplaceMaterials();

        // To materialize the object use this:
        //dissolver.SetState(Dissolver.DissolverState.Dissolved);
        //dissolver.MaterializeDissolve();

        // Or this:
        dissolver.Materialize();

        // Called automatically after Materialize() ends when automaticallyRestoreDefaultMaterials is true.
        //dissolver.RestoreDefaultMaterials();
    }
}
