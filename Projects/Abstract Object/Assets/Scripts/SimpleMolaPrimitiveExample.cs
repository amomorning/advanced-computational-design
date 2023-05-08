using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;

[ExecuteInEditMode]
public class SimpleMolaPrimitiveExample : MonoBehaviour
{
    [Range(0, 10)]
    public float length;
    private Mesh unityMesh;
    private MolaMesh molaMesh;

    private void Start()
    {
        // create unity mesh, add mesh filter, mesh renderer
        unityMesh = InitMesh();
        UpdateGeometry();
    }
    private void OnValidate()
    {
        UpdateGeometry();
    }
    private void UpdateGeometry()
    {
        // create mola mesh
        molaMesh = MeshFactory.CreateBox(0, 0, 0, length, length, length);

        // convert mola mesh to unity mesh
        if(unityMesh != null)
        {
            molaMesh.FillUnityMesh(unityMesh);
        }
    }

    private Mesh InitMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = this.gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        }
        meshRenderer.material = new Material(Shader.Find("Standard"));
        return mesh;
    }

}
