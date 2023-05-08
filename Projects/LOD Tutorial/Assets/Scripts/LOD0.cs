using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;
using System.Linq;

public class LOD0 : MolaMonoBehaviour
{
    [Range(1, 5)]
    public float height = 2.0f;
    [Range(0, 1)]
    public float ratio = 0.3f;
    [Range(0, 10000)]
    public int seed = 100;

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

    public List<int> Shuffle(List<int> arr, int count=100) {
        Random.InitState(seed);
        int n = arr.Count();
        for (int i = 0; i < count; ++ i) {
            int j = Random.Range(0, n);
            int k = Random.Range(0, n);
            int v = arr[j];
            arr[j] = arr[k];
            arr[k] = v;
        }
        return arr;
    }

    public override void UpdateGeometry()
    {
        List<MolaMesh> tmp = GetMeshFromLOD();

        if (tmp.Count != 0)
        {
            MolaMesh wall = tmp[0];
            MolaMesh roof = tmp[1];

            int count = (int)(wall.FacesCount() * ratio);

            List<int> faceids = Shuffle(Enumerable.Range(0, wall.FacesCount()).ToList());

            MolaMesh balcony = wall.CopySubMesh(faceids.Take(count).ToList());
            balcony = MeshSubdivision.SubdivideMeshExtrude(balcony, height);
            roof = MeshSubdivision.SubdivideMeshExtrudeTapered(roof, 1.0f);

            molaMeshes = new List<MolaMesh>() { wall, roof, balcony };
            FillUnitySubMesh(molaMeshes);
            ColorSubMeshRandom();
            UpdateLOD();
        }
    }

}
