using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;

public class Building_I : MolaMonoBehaviour
{
    [Range(3, 200)]
    public float dX = 10;
    [Range(3, 200)]
    public float dY = 15;
    [Range(3, 500)]
    public float dZ = 16;
    public MolaMesh startMesh;

    [Range(0, 3)]
    public float extrudeHeight = 2;

    public override void UpdateGeometry()
    {
        // 2 methods: get input startmesh or create a quad with dX, dY
        if (startMesh == null)
        {
            startMesh = MeshFactory.CreateQuad(dX, dY);
        }

        // create 1st level of LOD mesh and assign material
        MolaMesh volume = startMesh;
        volume = MeshSubdivision.SubdivideMeshExtrude(volume, dZ);
        AddLODMesh(volume, "Blue"); // you could add a single mesh and one material name of the material in Resources folder

        // create 2nd level of LOD mesh and assign material
        MolaMesh roof = volume.CopySubMesh(volume.FacesCount() - 1); // copy last face
        MolaMesh wall = volume.CopySubMesh(volume.FacesCount() - 1, true); // copy faces of the rest

        wall = MeshSubdivision.SubdivideMeshGrid(wall, 5, 5);
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

        MolaMesh window = wall.CopySubMeshByModulo(4, 5); // get every 5th face
        wall = wall.CopySubMeshByModulo(4, 5, true); // get the rest

        // you could add a mesh list and a material name list of the material in Resources folder
        AddLODMeshes(new List<MolaMesh> { wall, roof, window }, new List<string> { "Blue", "Green", "Glass" });

        // this step to add all lod mola meshes to unity object
        AddLODsToObject();
    }
}
