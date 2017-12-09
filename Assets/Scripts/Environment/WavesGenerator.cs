using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesGenerator : MonoBehaviour {

    public GameObject waveObj;

    List<Vector2> coast = new List<Vector2>();

    public IEnumerator GenerateWaves(WorldData world) {
        for(int y = 0; y < TerrainGenerator.size; y++) {
            for(int x = 0; x < TerrainGenerator.size / 3; x++) {
                Vector2 coords = new Vector2(x, y);
                if(world.voxels[coords].isOcean && world.AdjacentProperty(coords, "isLand", false)) {
                    for(int i = 1; i < 3; i++) {
                        if (!world.voxels[coords + Vector2.left * i].isOcean)
                            continue;
                        coast.Add(new Vector2(x, y));
                    }
                }
            }
        }

        while (true) {
            Vector2 coords = coast[(int)Random.Range(0, coast.Count)];
            Instantiate(waveObj, Util.CoordsToVector3(coords), Quaternion.identity, this.transform);
            yield return new WaitForSeconds(.5f);
        }
    }
}
