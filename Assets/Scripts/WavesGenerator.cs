using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesGenerator : MonoBehaviour {

    public TerrainGenerator terrainGenerator;
    public GameObject waveObj;

    List<Voxel> coast = new List<Voxel>();

    private IEnumerator Start() {
        while (!terrainGenerator.finished)
            yield return null;

        foreach(Voxel voxel in terrainGenerator.grid.Values) {
            if (voxel.IsTypeNeighboringType(0, 1) && terrainGenerator.grid[voxel.coords + Vector2.right].type != 0) {
                if (voxel.coords.x < TerrainGenerator.size / 3) {
                    for (int x = 1; x < 3; x++) {
                        if (terrainGenerator.grid[voxel.coords + Vector2.left * x].type != 0)
                            continue;
                    }
                    coast.Add(voxel);
                }
            }
        }

        while (true) {
            Voxel voxel = coast[(int)Random.Range(0, coast.Count)];
            Instantiate(waveObj, new Vector3(voxel.transform.position.x, 0, voxel.transform.position.z), Quaternion.identity, this.transform);
            yield return new WaitForSeconds(.5f);
        }
    }
}
