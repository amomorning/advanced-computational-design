using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;

public class LOD2_I : MolaMonoBehaviour
{
    [Range(5, 100)]
    public float dX = 8;
    [Range(5, 100)]
    public float dY = 5;
    [Range(5, 100)]
    public float dZ = 9;


    void Start()
    {
        InitMesh();
        UpdateGeometry();
    }

    private void OnValidate()
    {
        UpdateGeometry();
    }

    public override void UpdateGeometry()
    {
        Random.InitState(0);
        #region REPLACE THIS PART WITH YOUR OWN DESIGN
        // create mola mesh for current LOD level
        MolaMesh floor = MeshFactory.CreateSingleQuad(dX / 2, -dY / 2, 0, dX / 2, dY / 2, 0, -dX / 2, dY / 2, 0, -dX / 2, -dY / 2, 0, false);

        MolaMesh wall = new MolaMesh();
        MolaMesh roof = new MolaMesh();
        floor = MeshSubdivision.SubdivideMeshExtrude(floor, dZ);

        roof = floor.CopySubMesh(4, false);
        wall = floor.CopySubMesh(new List<int>() { 0, 1, 2, 3 });

        // store meshes in a list for next LOD level
        molaMeshes = new List<MolaMesh>() { wall, roof };
        #endregion

        // flip mesh YZ only for unity visualization

        // visualize mesh in current LOD level 
        FillUnitySubMesh(molaMeshes, true);
        ColorSubMeshRandom();

        // 02 update LOD1 and LOD2
        UpdateLOD();
    }
}
