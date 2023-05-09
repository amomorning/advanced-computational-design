using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;

public class PlotSubdivide_I : MolaMonoBehaviour
{
    [Range(10, 300)]
    public float dX = 206;
    [Range(10, 300)]
    public float dY = 120;
    [Range(6, 50)]
    public float streetwidth = 7;


    override public void UpdateGeometry()
    {
        // clear previously generated plots and buildings
        ClearChildrenImmediate();

        // subdivide plot into smaller plots and street
        MolaMesh plot = MeshFactory.CreateQuad(dX, dY);
        plot = MeshSubdivision.SubdivideMeshOffsetPerEdge(plot, new float[] { -2f, -1f, -4f, -6f });
        plot = MeshSubdivision.SubdivideMeshGrid(plot, 1, 3);

        plot = MeshSubdivision.SubdivideMeshOffset(plot, streetwidth * -0.5f);

        MolaMesh plots = plot.CopySubMeshByModulo(3, 4); // get every 4th face
        MolaMesh street = plot.CopySubMeshByModulo(3, 4, true); // get the rest faces

        List<MolaMesh> meshes = new List<MolaMesh> { plots, street };
        List<string> materials = new List<string> { "Green", "Grey" };

        // new method of add mola mesh to unity object with LOD embedded
        AddLODMeshes(meshes, materials);
        AddLODsToObject();

        // for each face of plots, create a new game object, attach your building script
        //Debug.Log(transform.position);
        for (int i = 0; i < plots.FacesCount(); i++)
        {
            GameObject buildingObject = new GameObject();
            buildingObject.transform.parent = this.transform;
            buildingObject.transform.localPosition = new Vector3(0, 0, 0); // if the plot is not at origin, need to reset child local position.

            var buildingscript = buildingObject.AddComponent<Building_I>();
            buildingscript.startMesh = plots.CopySubMesh(i);
            buildingscript.dZ = Random.Range(5, 100);
        }
    }
}
