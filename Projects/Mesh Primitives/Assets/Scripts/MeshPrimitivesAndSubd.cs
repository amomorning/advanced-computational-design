using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;
using System.Reflection;


public enum myEnum
{
    Box,
    Sphere,
    Cone,
    Torus,
    Tetrahedron,
    Icosahedron,
    Dodecahedron,
    Octahedron,
    RhombicDodecahedron
}
public class MeshPrimitivesAndSubd : MonoBehaviour
{
    public myEnum myDropDown = new myEnum();

    [Range(0, 10)]
    public float paraA = 1;
    [Range(0, 10)]
    public float paraB = 1;
    [Range(0, 10)]
    public float paraC = 1;
    [Range(0, 1)]
    public float colorValue = 0;
    [Range(0, 5)]
    public int subdInteration = 1;
    [Range(0, 10)]
    public float subdLength = 0;


    private Mesh mesh;

    private void OnValidate()
    {
        InitMesh();

        MolaMesh molaMesh = new MolaMesh();

        // create mola mesh primitive from the selection of dropdown menu
        switch (myDropDown)
        {
            case myEnum.Box:
                molaMesh = MeshFactory.CreateBox(0, 0, 0, paraA, paraA, paraA);
                break;            
            case myEnum.Cone:
                molaMesh = MeshFactory.CreateCone(0, paraA, paraB, paraC, 6, true, true);
                break;
            case myEnum.Sphere:
                molaMesh = MeshFactory.CreateSphere(paraA);
                break;            
            case myEnum.Torus:
                molaMesh = MeshFactory.CreateTorus(paraA, paraB);
                break;            
            case myEnum.Tetrahedron:
                molaMesh = MeshFactory.CreateTetrahedron(paraA, 0, 0, 0);
                break;
            case myEnum.Dodecahedron:
                molaMesh = MeshFactory.CreateDodecahedron(paraA, 0, 0, 0);
                break;
            case myEnum.Icosahedron:
                molaMesh = MeshFactory.CreateIcosahedron(paraA, 0, 0, 0);
                break;
            case myEnum.Octahedron:
                molaMesh = MeshFactory.CreateOctahedron(paraA);
                break;
            case myEnum.RhombicDodecahedron:
                molaMesh = MeshFactory.CreateRhombicDodecahedron(paraA);
                break;
        }

        // apply mesh subdivision
        for (int i = 0; i < subdInteration; i++)
        {
            //molaMesh = MeshSubdivision.SubdivideMeshCatmullClark(molaMesh);
            float p = Random.Range(0f, 1f);
            Debug.Log(p.ToString());
            if (p > 0.7) {
                Debug.Log("Extrude");
                molaMesh = MeshSubdivision.SubdivideMeshExtrude(molaMesh, subdLength);
            } else if(p > 0.3) {
                Debug.Log("Split Frame");
                molaMesh = MeshSubdivision.SubdivideMeshSplitFrame(molaMesh, p);
            }
            else {
                Debug.Log("Extrude Tapered");
                molaMesh = MeshSubdivision.SubdivideMeshExtrudeTapered(molaMesh, subdLength);
            }
        }
        
        //molaMesh.SeparateVertices();
        molaMesh.FillUnityMesh(mesh);
    }

    private void InitMesh()
    {

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (null == meshFilter)
        {
            meshFilter = this.gameObject.AddComponent<MeshFilter>();
        }
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshFilter.mesh = mesh;

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            renderer = this.gameObject.AddComponent<MeshRenderer>();
        }
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.sharedMaterial.color = Color.HSVToRGB(colorValue, 1, 1);
    }
}
