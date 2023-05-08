using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;

public class BlockSubdivisionA : MolaMonoBehaviour
{
    [Range(0, 10000)]
    public int seed = 0;
    [Range(4, 15)]
    public float roadWidth = 8;
    [Range(1, 5)]
    public int nX = 3;
    [Range(1, 5)]
    public int nY = 3;
    void Start()
    {
        InitMesh();
        UpdateGeometry();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += UpdateGeometry;
#endif
    }

    public override void UpdateGeometry()
    {
        Random.InitState(seed);
        Mesh refMesh = transform.parent.GetComponent<MeshFilter>().sharedMesh;
        if (refMesh == null) return;

        // convert it to mola mesh
        List<Vec3> vertices = new List<Vec3>();
        foreach (var v in refMesh.vertices)
        {
            vertices.Add(new Vec3(v.x, v.z, 1));
        }

        MolaMesh block = MeshFactory.CreateSingleFace(vertices);

        // subdivide blocks
        //block = MeshSubdivision.SubdivideMeshExtrudeToPointCenter(block, 0);
        //block = MeshSubdivision.SubdivideMeshGrid(block, 2, 2);

        block = MeshSubdivision.SubdivideMeshGrid(block, nX, nY);

        MolaMesh plot1 = block.CopySubMesh(0);
        MolaMesh otherPlots = block.CopySubMesh(0, true);

        plot1 = MeshSubdivision.SubdivideMeshGrid(plot1, 2, 2);

        block = plot1;
        block.AddMesh(otherPlots);

        // make road
        block = MeshSubdivision.SubdivideMeshSplitFrame(block, roadWidth * 0.5f);

        // seperate plots and road by face normal
        bool[] filterMask = new bool[block.FacesCount()];
        for (int i = 0; i < block.FacesCount(); i++)
        {
            if (block.FaceAngleVertical(i) >= 0)
            {
                filterMask[i] = true;
            }
        }

        MolaMesh plots = block.CopySubMesh(filterMask);
        for (int i = 0; i < filterMask.Length; i++)
        {
            filterMask[i] = !filterMask[i];
        }
        MolaMesh road = block.CopySubMesh(filterMask);
        road.FlipFaces();

        molaMeshes = new List<MolaMesh> { plots, road };
        FillUnitySubMesh(molaMeshes, true);
        ColorSubMeshRandom();

        InstantiatePrefab(plots);
    }

    private void InstantiatePrefab(MolaMesh mesh)
    {
        ClearChildrenImmediate();

        for (int i = 0; i < mesh.FacesCount(); i++) {
            var prefabLoad = Resources.Load<GameObject>("LOD_W");
            GameObject LODPrefab = Instantiate(prefabLoad,transform);
            LODPrefab.GetComponent<MolaLOD>().startMesh = mesh.CopySubMesh(i);
            LODPrefab.GetComponent<MolaLOD>().DimZ = Random.Range(6, 100);
        }

    }
}
