using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;

public enum BuildingType
{
    None,
    Office,
    House,
    Museum,
    Park
}

public class Building_I : MolaMonoBehaviour
{
    public BuildingType buildingType = new();

    [Range(3, 20)]
    public float dX = 16;
    [Range(3, 20)]
    public float dY = 16;
    [Range(3, 500)]
    public float dZ = 16;
    public MolaMesh startMesh;

    [Range(0, 3)]
    public float extrudeHeight = 2;

    public MolaMesh scaledMesh(MolaMesh mesh, float dx, float dy, float dz)
    {
        MolaMesh ret = mesh.Copy();
        Vec3 center = new Vec3();
        for (int i = 0; i < mesh.VertexCount(); ++i)
        {
            center += mesh.Vertices[i];
        }
        center /= mesh.VertexCount();

        ret.Translate(-center.x, -center.y, -center.z);
        ret.Scale(dx, dy, dz);
        ret.Translate(center.x, center.y, center.z);

        return ret;
    }

    public override void UpdateGeometry()
    {
        // 2 methods: get input startmesh or create a quad with dX, dY
        MolaMesh volume = new MolaMesh();
        if (startMesh == null)
        {
            startMesh = MeshFactory.CreateQuad(dX, dY);
            volume = startMesh.Copy();

        }
        else
        {
            volume = scaledMesh(startMesh, dX / 20, dY / 20, 1f);
        }

        //if (buildingType == BuildingType.None) { buildingType = BuildingType.Office; }
        if (buildingType != BuildingType.None || buildingType != BuildingType.Park)
        {
            volume = MeshSubdivision.SubdivideMeshExtrude(volume, dZ);
            AddLODMesh(volume, "Blue"); // you could add a single mesh and one material name of the material in Resources folder

        }

        MolaMesh wall = new MolaMesh();
        MolaMesh roof = new MolaMesh();
        MolaMesh window = new MolaMesh();
        switch (buildingType)
        {
            case BuildingType.Office:

                // create 2nd level of LOD mesh and assign material
                roof = volume.CopySubMesh(volume.FacesCount() - 1); // copy last face
                wall = volume.CopySubMesh(volume.FacesCount() - 1, true); // copy faces of the rest

                wall = MeshSubdivision.SubdivideMeshSplitGridAbs(wall, 5f, 5f);
                List<int> indexList = new List<int>();
                Random.InitState(0);
                for (int i = 0; i < wall.Faces.Count; i++)
                {
                    if (Random.value > 0.5f)
                    {
                        indexList.Add(i);
                    }
                }

                MolaMesh newWall = wall.CopySubMesh(indexList); // copy faces by index list
                wall = wall.CopySubMesh(indexList, true); // copy the inverted index list

                newWall = MeshSubdivision.SubdivideMeshExtrude(newWall, extrudeHeight);

                MolaMesh newRoof = newWall.CopySubMeshByNormalZ(0.1f, 1.1f); // face facing up with normalz 1, facing down with normalz -1, vertial 0 
                newWall = newWall.CopySubMeshByNormalZ(0.1f, 1.1f, false, true); // copy the rest

                roof.AddMesh(newRoof);
                wall.AddMesh(newWall);

                wall = MeshSubdivision.SubdivideMeshLinearSplitQuad(wall, 0.5f, 2f);
                wall = MeshSubdivision.SubdivideMeshExtrudeTapered(wall, 0, 0.2f);

                window = wall.CopySubMeshByModulo(4, 5); // get every 5th face
                wall = wall.CopySubMeshByModulo(4, 5, true); // get the rest
                break;
            case BuildingType.Museum:
                volume = volume.CopySubMesh(volume.FacesCount() - 1);
                volume.Translate(0, 0, -dZ);


                float cur = 0.0f;
                while (cur < dZ)
                {
                    float scale = Random.Range(0.2f, 1.1f);
                    float height = 10f * scale;

                    MolaMesh layer = scaledMesh(volume, scale, scale, 1f);
                    MolaMesh floor = layer.Copy();
                    floor.FlipFaces();
                    floor.Translate(0, 0, cur);
                   
                    
                    roof.AddMesh(floor);

                    layer = MeshSubdivision.SubdivideMeshExtrude(layer, height);
                    layer.Translate(0, 0, cur);
                    MolaMesh roofT = layer.CopySubMesh(layer.FacesCount() - 1); // copy last face
                    MolaMesh wallT = layer.CopySubMesh(layer.FacesCount() - 1, true); // copy faces of the rest

                    wallT = MeshSubdivision.SubdivideMeshSplitGridAbs(wallT,6f, 10f);

                    wallT = MeshSubdivision.SubdivideMeshOffsetPerEdge(wallT, new float[] { -0.2f, -2f, -height*0.9f, -2f });

                    MolaMesh windowT = wallT.CopySubMeshByModulo(2, 5, true);
                    wallT = wallT.CopySubMeshByModulo(2, 5);

                    roofT = MeshSubdivision.SubdivideMeshExtrude(roofT, 0.5f, true);
                    roof.AddMesh(roofT);
                    wall.AddMesh(wallT);
                    window.AddMesh(windowT);
                    cur += height;
                }

                break;
            case BuildingType.House:

                roof = volume.CopySubMesh(volume.FacesCount() - 1); // copy last face
                wall = volume.CopySubMesh(volume.FacesCount() - 1, true); // copy faces of the rest
                roof = MeshSubdivision.SubdivideMeshSplitRoof(roof, 2f);
                wall = MeshSubdivision.SubdivideMeshSplitGridAbs(wall, 7.2f, 3.6f);

                wall = MeshSubdivision.SubdivideMeshOffsetPerEdge(wall, new float[] { -0.2f, -2f, -2f, -1f });
                window = wall.CopySubMeshByModulo(4, 5); // get every 5th face
                wall = wall.CopySubMeshByModulo(4, 5, true); // get the rest
                break;

            case BuildingType.Park:
                break;
            default:
                break;
        }

        AddLODMeshes(new List<MolaMesh> { wall, roof, window }, new List<string> { "White", "Concrete", "Glass" });
        AddLODsToObject();

    }
}
