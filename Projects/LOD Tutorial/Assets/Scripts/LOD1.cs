using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;

public class LOD1 : MolaMonoBehaviour
{
    [Range(1, 10)]
    public int nX = 3;
    [Range(1, 10)]
    public int nY = 3;

    // Start is called before the first frame update
    void Start()
    {
        InitMesh();
        UpdateGeometry();
    }

    // Update is called once per frame
    private void OnValidate()
    {
        UpdateGeometry();
    }

    public override void UpdateGeometry()
    {
        List<MolaMesh> tmp = GetMeshFromLOD();

        if (tmp.Count != 0) {
            MolaMesh wall = tmp[0];
            MolaMesh roof = tmp[1];

            wall = MeshSubdivision.SubdivideMeshGrid(wall, nX, nY);
            roof = MeshSubdivision.SubdivideMeshGrid(roof, 1, 2);
            roof = MeshSubdivision.SubdivideMeshExtrudeToPointCenter(roof, 6.0f);

            molaMeshes = new List<MolaMesh>() { wall, roof };
            FillUnitySubMesh(molaMeshes);
            ColorSubMeshRandom();
            UpdateLOD();
        }

    }
}
