using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;

public class LOD2 : MolaMonoBehaviour
{
    [Range(3, 20)]
    public float length = 10;
    [Range(3, 20)]
    public float width = 8;
    [Range(3, 20)]
    public float height = 4;

    // Start is called before the first frame update
    void Start()
    {
        InitMesh();
        UpdateGeometry();
    }

    private void OnValidate()
    {
        UpdateGeometry();
    }
    // Update is called once per frame
    void Update()
    {

    }

    public override void UpdateGeometry()
    {
        MolaMesh floor = MeshFactory.CreateSingleQuad(length / 2, 0, -width / 2, length / 2, 0, width / 2, -length / 2, 0, width / 2, -length / 2, 0, -width / 2, true);
        floor = MeshSubdivision.SubdivideMeshExtrude(floor, height);

        MolaMesh roof = floor.CopySubMesh(4);
        MolaMesh wall = floor.CopySubMesh(new List<int> { 0, 1, 2, 3 });

        molaMeshes = new List<MolaMesh>() { wall, roof };
        FillUnitySubMesh(molaMeshes);
        ColorSubMeshRandom();
        UpdateLOD();
    }   

}
