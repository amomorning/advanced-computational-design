using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;
using System.Linq;

public class LOD1_I : MolaMonoBehaviour
{
    [Header("Roof Settings")]
    [Range(0.9f, 2f)]
    public float roofX = 1.3f;
    [Range(1.2f, 3f)]
    public float roofY = 1.8f;
    [Range(1.2f, 3f)]
    public float roofHeight = 1.5f;

    [Header("Window S Settings")]
    public bool overallWindowS = false;
    [Range(0.2f, 1f)]
    public float scaleWindowS = 0.4f;
    [Range(3f, 6f)]
    public float extrudeWindowS = 4f;


    [Header("Window W Settings")]
    public bool overallWindowW = false;
    [Range(0.2f, 1f)]
    public float scaleXWindowW = 0.4f;
    [Range(0.2f, 1f)]
    public float scaleZWindowW = 0.8f;
    [Range(0.6f, 3f)]
    public float heightWindowW = 0.6f;
    [Range(2f, 8f)]
    public float extrudeWindowW = 2.5f;

    [Header("Window E Settings")]
    public bool overallWindowE = true;
    [Range(0.2f, 1f)]
    public float scaleXWindowE = 0.5f;
    [Range(0.2f, 1f)]
    public float scaleZWindowE = 0.6f;
    [Range(0.6f, 3f)]
    public float heightWindowE = 1.5f;
    [Range(2f, 8f)]
    public float extrudeWindowE = 2.5f;





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
        // 01 get mola meshes from LOD2
        molaMeshes = GetMeshFromLOD();

        MolaMesh wall = new MolaMesh();
        MolaMesh roof = new MolaMesh();
        if (molaMeshes.Count != 0)
        {
            wall = molaMeshes[0];
            roof = molaMeshes[1];
        }

        // 02 operate in the current level 
        roof.Scale(roofX, roofY, 0.96f);
        roof = MeshSubdivision.SubdivideMeshSplitRoof(roof, roofHeight);
        if (roof.FacesCount() > 0) { 
            roof = roof.CopySubMesh(new List<int>() { 1, 3 });
        }

        MolaMesh window = new MolaMesh();
        if(wall.FacesCount() > 0) {
            // 01 window S
            MolaMesh windowS = wall.CopySubMesh(new List<int>() {0, 2});
            MolaMesh windowWE = wall.CopySubMesh(new List<int>() { 1, 3 });
            wall = new MolaMesh();

            if(!overallWindowS) { 
                windowS = MeshSubdivision.SubdivideMeshGrid(windowS, 1, 2);
            }
            wall.AddMesh(windowS);

            if(!overallWindowS) { 
                window = windowS.CopySubMesh(3, false);
            } else { 
                window = windowS.CopySubMesh(1, false);
            }
            window = MeshSubdivision.SubdivideMeshSplitFrame(window, scaleWindowS);
            window = window.CopySubMesh(8, false);
            window = MeshSubdivision.SubdivideMeshExtrude(window, extrudeWindowS * (roofX - 1f));

            // 02 window W/E
            windowWE.Scale(roofX, 1f, 1f);
            wall.AddMesh(windowWE);


            MolaMesh windowE = new MolaMesh();

            if (!overallWindowE) { 
                windowWE = MeshSubdivision.SubdivideMeshGrid(windowWE, 1, 2);
                windowE = windowWE.CopySubMesh(3, false);
            } else { 
                windowE = windowWE.CopySubMesh(1, false);
            }

            windowE.Scale(scaleXWindowE, 1f, scaleZWindowE);
            windowE.Translate(0, 0, heightWindowE);

            windowE = MeshSubdivision.SubdivideMeshExtrude(windowE, extrudeWindowE * (roofY-1f) - 0.3f);
            window.AddMesh(windowE);

            MolaMesh windowW = new MolaMesh();
            if (overallWindowE && !overallWindowW) { 
                windowWE = MeshSubdivision.SubdivideMeshGrid(windowWE, 1, 2);
                windowW = windowWE.CopySubMesh(1, false);
            } else { 
                windowW = windowWE.CopySubMesh(0, false);
            }

            windowW.Scale(scaleXWindowW, 1f, scaleZWindowW);
            windowW.Translate(0f, 0f, heightWindowW);

            windowW = MeshSubdivision.SubdivideMeshExtrude(windowW, extrudeWindowW * (roofY-1f) - 0.3f);
            window.AddMesh(windowW);

        }

        molaMeshes = new List<MolaMesh>() { wall, roof, window};
        #endregion

        // visualize current 
        MolaMesh wallE = MeshSubdivision.SubdivideMeshExtrude(wall, 0.01f);
        wall.FlipFaces();
        wallE.AddMesh(wall);

        MolaMesh roofE = MeshSubdivision.SubdivideMeshExtrude(roof, 0.01f);
        roof.FlipFaces();
        roofE.AddMesh(roof);
        List<MolaMesh> renderMeshes = new List<MolaMesh>() { wallE, roofE, window};

        FillUnitySubMesh(renderMeshes, true);
        ColorSubMeshRandom();

        // 03 update LOD2
        UpdateLOD();
    }
}
