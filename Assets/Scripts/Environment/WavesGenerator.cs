using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesGenerator : MonoBehaviour {

    public GameObject waveObj;

    List<Vector2> coast = new List<Vector2>();

    public IEnumerator GenerateWaves(World world) {
        for(int y = 0; y < TerrainGenerator.size; y++) {
            for(int x = 0; x < TerrainGenerator.size / 3; x++) {
                if(world.PropertyAdjacentProperty(x, y, 1, 0, false) && !world.GetProperty(x + 1, y, "isOcean")) {
                    for(int i = 1; i < 3; i++) {
                        if (!world.GetProperty(x - i, y, "isOcean"))
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
