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
    [Range(4, 18)]
    public float streetwidth = 7;

    [Range(0, 5)]
    public int numMuseum = 1;
    [Range(0.1f, 0.8f)]
    public float ratioHouse = 0.4f;


    override public void UpdateGeometry()
    {
        // clear previously generated plots and buildings
        ClearChildrenImmediate();

        // subdivide plot into smaller plots and street
        MolaMesh plot = MeshFactory.CreateQuad(dX, dY);
        plot = MeshSubdivision.SubdivideMeshOffsetPerEdge(plot, new float[] { -50f, -40f, -40f, -60f });
        plot = MeshSubdivision.SubdivideMeshSplitRelative(plot, 0, 0.6f, 0.9f, 0.3f, 0.5f);

        MolaMesh newplot = new MolaMesh();
        for(int i = 0; i < plot.FacesCount(); ++ i) {
            if (plot.FaceArea(i) > 6000) {
                List<Vec3[]> new_faces_vertices = FaceSubdivision.Grid(plot.FaceVertices(i), 2, 2);
                newplot.AddFaces(new_faces_vertices);
            } else { 
                newplot.AddFace(plot.FaceVertices(i));
            }
        }

        plot = MeshSubdivision.SubdivideMeshOffset(newplot, streetwidth * -0.5f);

        MolaMesh plots = plot.CopySubMeshByModulo(4, 5); // get every 4th face
        MolaMesh street = plot.CopySubMeshByModulo(4, 5, true); // get the rest faces


        List<MolaMesh> meshes = new List<MolaMesh> { plots, street };
        List<string> materials = new List<string> { "Green", "Grey"};

        // new method of add mola mesh to unity object with LOD embedded
        AddLODMeshes(meshes, materials);
        AddLODsToObject();

        // for each face of plots, create a new game object, attach your building script
        //Debug.Log(transform.position);

        List<System.Tuple<int, float>> ls = new List<System.Tuple<int, float>>();
        for (int i = 0; i < plots.FacesCount(); ++ i) {
            ls.Add(new System.Tuple<int, float>(i, -faceArea(plots.FaceVertices(i)) / plots.FaceBoundingBox(i).GetAreaXY()));
        }
        ls.Sort((x, y) => x.Item2.CompareTo(y.Item2));

        int[] st = new int[numMuseum];
        for (int i = 0; i < numMuseum; ++ i) {
            st[i] = ls[i].Item1;
        }


        for (int i = 0; i < plots.FacesCount(); i++)
        {
            GameObject buildingObject = new GameObject();
            buildingObject.transform.parent = this.transform;
            buildingObject.transform.localPosition = new Vector3(0, 0, 0); // if the plot is not at origin, need to reset child local position.



            var buildingscript = buildingObject.AddComponent<Building_I>();
            buildingscript.startMesh = plots.CopySubMesh(i);
            buildingscript.dZ = Random.Range(5, 100);

            float rng = Random.Range(0f, 1f);
            if (rng < ratioHouse) {
                buildingscript.buildingType = BuildingType.House;
            } else { 
                buildingscript.buildingType = BuildingType.Office;
            }


            bool flag = false;
            foreach (var x in st) {
                if (i == x) flag = true;
            }
            if (flag) { 
                buildingscript.buildingType = BuildingType.Museum;
            }
        }
    }

    public float faceArea(Vec3[] vertices) {
        float ret = 0;
        for(int i = 1; i < vertices.Length-1; ++ i) { 
          Vec3 a = vertices[i] - vertices[0];
          Vec3 b = vertices[i+1] - vertices[0];
            ret += Mola.Mathf.Abs(crossXY(a, b) / 2);
        }
        return ret;
    }

    public float crossXY(Vec3 a, Vec3 b)
    {
        return a.x * b.y - a.y * b.x;
    }
}
