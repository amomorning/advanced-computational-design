using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;

public class VoxelSpace : MolaMonoBehaviour
{
    [Range(1, 100)]
    public int nX = 8;
    [Range(1, 100)]
    public int nY = 4;
    [Range(1, 100)]
    public int nZ = 10;
    [Range(1, 100)]
    public int cZ = 20;
    [Range(0, 1)]
    public float density = 0.2f;
    [Range(0, 10000)]
    public int seed = 100;
    // Start is called before the first frame update
    void Start()
    {
        InitMesh();
        UpdateGeometry();
        
    }

    // Update is called once per frame
    public void OnValidate()
    {
        UpdateGeometry();
    }

    public override void UpdateGeometry()
    {
        int maxZ = UnityEngine.Mathf.Max(cZ, nZ);
        UnityEngine.Random.InitState(seed);
        MolaGrid<bool> grid = new MolaGrid<bool>(nX, nY, maxZ);
        for (int x = 0; x < nX; ++ x) { 
            for (int y = 0; y < nY; ++ y) { 
                for (int z = 0; z < nZ; ++ z) {
                    float randomValue = UnityEngine.Random.Range(0.0f, 1.0f);
                    grid.SetValue(x, y, z, randomValue > 1.0f-density);
                } 
            }
        }

        for (int z = nZ-1; z < maxZ; ++ z) { 
            for (int x = 1; x < nX - 1; ++ x) { 
                for (int y = 1; y < nY-1; ++ y) {
                    int neighbours = 0;
                    for (int cX = x - 1; cX <= x + 1; ++cX)
                    {
                        for (int cY = y - 1; cY <= y + 1; ++cY)
                        {
                            if (grid.GetValue(cX, cY, z))
                            {
                                neighbours++;
                            }

                        }
                    }

                    bool cellValue = grid.GetValue(x, y, z);

                    if (cellValue && neighbours < 2) { 
                        grid.SetValue(x, y, z + 1, false);
                    } else if (cellValue && (neighbours == 2 || neighbours ==3 )) { 
                        grid.SetValue(x, y, z + 1, false);
                    } else if (!cellValue && (neighbours == 3 || neighbours == 4)) { 
                        grid.SetValue(x, y, z + 1, true);
                    } else if (!cellValue) { 
                        grid.SetValue(x, y, z + 1, false);
                    }


                }
            }
        }

        MolaMesh mesh = UtilsGrid.VoxelMesh(grid);
        mesh.FlipYZ();
        mesh.FlipFaces();

        molaMeshes = new List<MolaMesh> { mesh };
        FillUnitySubMesh(molaMeshes);
        ColorSubMeshRandom();
    }
}
