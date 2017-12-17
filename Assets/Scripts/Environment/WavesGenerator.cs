using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesGenerator : MonoBehaviour {

    public GameObject waveObj;

    List<Coords> coast = new List<Coords>();

    public IEnumerator GenerateWaves(WorldData world) {
        for(int y = 0; y < TerrainGenerator.size; y++) {
            for(int x = 0; x < TerrainGenerator.size / 3; x++) {
                Coords coords = new Coords(x, y);
                if(world.voxelData[x, y].isOcean && Util.AdjacentProperty(Util.CoordsToVoxels(world.voxelData, Util.GetAdjacent(coords, false)), "isLand")) {
                    for(int i = 1; i < 3; i++) {
                        if (!world.voxelData[x - i, y].isOcean)
                            continue;
                        coast.Add(new Coords(x, y));
                    }
                }
            }
        }

        while (true) {
            Coords coords = coast[(int)Random.Range(0, coast.Count)];
            Instantiate(waveObj, Util.CoordsToVector3(coords), Quaternion.identity, this.transform);
            yield return new WaitForSeconds(.5f);
        }
    }
}
