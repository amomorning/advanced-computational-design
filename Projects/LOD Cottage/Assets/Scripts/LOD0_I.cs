using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;
using System.Linq;

public class LOD0_I : MolaMonoBehaviour
{
    [Range(10, 50)]
    public int roofTrussNum = 20;
    [Range(0.1f, 0.9f)]
    public float thick = 0.15f;
    [Range(1f, 2f)]
    public float scaleWindow = 1.2f;
    [Range(-3f, 3f)]
    public float heightWindow = 1f;


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
        // 01 get mola meshes from LOD1
        molaMeshes = GetMeshFromLOD();
        MolaMesh wall = new MolaMesh();
        MolaMesh roof = new MolaMesh();
        MolaMesh window = new MolaMesh();

        if (molaMeshes.Count != 0)
        {
            wall = molaMeshes[0];
            roof = molaMeshes[1];
            window = molaMeshes[2];
        }


        // 02 roof operation
        MolaMesh roofG = MeshSubdivision.SubdivideMeshGrid(roof, roofTrussNum, 1);
        roofG = MeshSubdivision.SubdivideMeshExtrudeTapered(roofG, thick);
        roof.FlipFaces();
        MolaMesh roofE = MeshSubdivision.SubdivideMeshExtrude(roof, thick, true);
        roof.AddMesh(roofG);
        roof.AddMesh(roofE);

        // 03 wall operation
        MolaMesh wallE = MeshSubdivision.SubdivideMeshExtrude(wall, thick);
        wall.FlipFaces();
        wall.AddMesh(wallE);

        //04 window operatio

        if(window.FacesCount() > 0) { 
            MolaMesh windowSupportV = window.CopySubMesh(new List<int> { 1, 3, 6, 8, 11, 13});
            MolaMesh windowSupportE = MeshSubdivision.SubdivideMeshExtrude(windowSupportV, thick);
            windowSupportV.FlipFaces();
            windowSupportV.AddMesh(windowSupportE);

            windowSupportV.Scale(1f, 1f, scaleWindow);
            windowSupportV.Translate(0f, 0f, -heightWindow);

            MolaMesh windowSupportH = window.CopySubMesh(new List<int> { 0, 2, 5, 7, 10, 12 });
            MolaMesh windowSupportHE = MeshSubdivision.SubdivideMeshExtrude(windowSupportH, thick);
            windowSupportH.FlipFaces();
            windowSupportH.AddMesh(windowSupportHE);

            wall.AddMesh(windowSupportV);
            wall.AddMesh(windowSupportH);
        }

        molaMeshes = new List<MolaMesh>() { wall, roof, window };
        #endregion

        // visualize current 
        FillUnitySubMesh(molaMeshes, true);
        ColorSubMeshRandom();
    }
}
