using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Procedural {

    public static float[,] NoiseMap(int seed, int size, float scale, AnimationCurve multiplier) {
        float[,] noiseMap = new float[size, size];

        System.Random rng = new System.Random(seed);
        Vector2 offset = new Vector2(rng.Next(-100000, 100000), rng.Next(-100000, 100000));

        float max = float.MinValue;
        float min = float.MaxValue;

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {

                float amplitude = 1f;
                float frequency = 1f;
                float h = 0f;

                for (int i = 0; i < 2; i++) {
                    float x_ = frequency * x / (size * scale) + offset.x;
                    float y_ = frequency * y / (size * scale) + offset.y;
                    float n = Mathf.PerlinNoise(x_, y_) * 2 - 1;
                    h += n * amplitude;
                    amplitude *= 2f;
                    frequency *= .5f;
                }
                noiseMap[x, y] = h;

                if (h > max)
                    max = h;
                else if (h < min)
                    min = h;
            }
        }

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                noiseMap[x, y] = multiplier.Evaluate(Mathf.InverseLerp(min, max, noiseMap[x, y]));
            }
        }

        return noiseMap;
    }

    public static float[,] FalloffMap(int size) {
        float[,] falloffMap = new float[size, size];

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                falloffMap[i, j] = Evaluate(value);
            }
        }

        return falloffMap;
    }

    static float Evaluate(float value) {
        float a = 3f;
        float b = 2.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }

    public static MeshData GenerateTerrain(World world) {
        int size = TerrainGenerator.size;

        MeshData meshData = new MeshData(size);

        List<MeshData.EdgeParameters> edges = new List<MeshData.EdgeParameters>();
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                bool isLand = world.GetProperty(x, y, "isLand");
                meshData.AddSquare(x, y, isLand);
                if (y < size - 1) {
                    if (isLand != world.GetProperty(x, y + 1, "isLand")) {
                        edges.Add(new MeshData.EdgeParameters(x, y + 1, 0, !world.GetProperty(x, y + 1, "isLand")));
                    }
                }
                if (x < size - 1) {
                    if (isLand != world.GetProperty(x + 1, y, "isLand")) {
                        edges.Add(new MeshData.EdgeParameters(x + 1, y, 1, !world.GetProperty(x + 1, y, "isLand")));
                    }
                }
            }
        }

        foreach(MeshData.EdgeParameters edge in edges) {
            meshData.AddEdge(edge);
        }

        return meshData;
    }

    public static Texture2D GenerateTexture(int seed, World worldState, float[,] landNoise, float[,] colorNoise) {
        int size = TerrainGenerator.size;

        Texture2D texture = new Texture2D(size * 2, size * 2);
        System.Random rng = new System.Random(seed);

        Color[] colorMap = new Color[4 * size * size];
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                int i = y * 2 * size + x;
                if(worldState.GetProperty(x, y, "isLand")) {
                    Color baseColor = Color.Lerp(Palette.LandMin, Palette.LandMax, rng.Next(0, 1000) / 1000f);
                    colorMap[i] = Color.Lerp(baseColor, Palette.Chartreuse, Mathf.Pow(colorNoise[x, y] * landNoise[x, y], 2));
                } else {
                    colorMap[i] = Palette.Water;
                }
            }
        }

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                int i = y * 2 * size + x;
                if (worldState.PropertyAdjacentProperty(x, y, 0, 1, true) && colorNoise[x, y] > .6f) {
                    colorMap[i] = Color.Lerp(colorMap[i], Palette.Sand, Mathf.Pow(colorNoise[x, y], 2));
                }
                else if(worldState.PropertyAdjacentProperty(x, y, 1, 0, true) || worldState.PropertyAdjacentProperty(x, y, 1, 2, true)) {
                    colorMap[i] = Color.Lerp(colorMap[i], Palette.Coast, .1f);
                }
            }
        }

        for(int y = 0; y < size * 2; y++) {
            for(int x = 0; x < size * 2; x++) {
                if(x >= size || y >= size) {
                    colorMap[y * 2 * size + x] = Palette.LandEdge;
                }
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }
}

public class MeshData {
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uvs = new List<Vector2>();
    public List<Vector3> normals = new List<Vector3>();

    int index;
    int size;

    public MeshData(int size) {
        this.size = size;
    }

    public void AddSquare(float x, float y, bool land) {
        float height = land ? 0f : -.1f;
        Vector3 a = new Vector3(x - .5f, height, y - .5f);
        Vector3 b = new Vector3(x - .5f, height, y + .5f);
        Vector3 c = new Vector3(x + .5f, height, y - .5f);
        Vector3 d = new Vector3(x + .5f, height, y + .5f);
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(b);
        vertices.Add(d);
        vertices.Add(c);
        uvs.Add(new Vector2((a.x + .5f) / (2 * size), (a.z + .5f) / (2 * size)));
        uvs.Add(new Vector2((b.x + .5f) / (2 * size), (b.z + .5f) / (2 * size)));
        uvs.Add(new Vector2((c.x + .5f) / (2 * size), (c.z + .5f) / (2 * size)));
        uvs.Add(new Vector2((b.x + .5f) / (2 * size), (b.z + .5f) / (2 * size)));
        uvs.Add(new Vector2((d.x + .5f) / (2 * size), (d.z + .5f) / (2 * size)));
        uvs.Add(new Vector2((c.x + .5f) / (2 * size), (c.z + .5f) / (2 * size)));
        Vector3 normal = Vector3.up;
        for (int i = 0; i < 2; i++) {
            if(land)
                normal = Quaternion.Euler(Random.Range(-10, 10), 0, Random.Range(-10, 10)) * Vector3.up;
            for (int j = 0; j < 3; j++) {
                triangles.Add(index++);
                normals.Add(normal);
            }
        }
    }

    public void AddEdge(EdgeParameters p) {
        float x = p.x;
        float y = p.y;
        int side = p.side;
        bool flip = p.flip;

        Vector3 a, b, c, d;
        a = b = c = d = Vector3.zero;
        switch(side) {
            case 0: //North
                a = new Vector3(x - .5f, -.1f, y - .5f);
                b = new Vector3(x - .5f, 0f, y - .5f);
                c = new Vector3(x + .5f, -.1f, y - .5f);
                d = new Vector3(x + .5f, 0f, y - .5f);
                break;
            case 1: //East
                a = new Vector3(x - .5f, -.1f, y - .5f);
                b = new Vector3(x - .5f, 0f, y - .5f);
                c = new Vector3(x - .5f, -.1f, y + .5f);
                d = new Vector3(x - .5f, 0f, y + .5f);
                break;
            default:
                break;
        }
        if (flip) {
            vertices.Add(c);
            vertices.Add(b);
            vertices.Add(a);
            vertices.Add(c);
            vertices.Add(d);
            vertices.Add(b);
            for (int i = 0; i < 6; i++) {
                triangles.Add(index++);
                uvs.Add(Vector2.one);
                if (side == 0) {
                    normals.Add(Vector3.forward);
                }
                else {
                    normals.Add(Vector3.left);
                }
            }
        } else {
            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);
            vertices.Add(b);
            vertices.Add(d);
            vertices.Add(c);
            for (int i = 0; i < 6; i++) {
                triangles.Add(index++);
                uvs.Add(Vector2.one);
                if(side == 0) { 
                    normals.Add(Vector3.back);
                } else {
                    normals.Add(Vector3.right);
                }
            }
        }
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh() {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray(),
            normals = normals.ToArray()
        };
        return mesh;
    }

    [System.Serializable]
    public struct EdgeParameters {
        public float x;
        public float y;
        public int side;
        public bool flip;

        public EdgeParameters(float x, float y, int side, bool flip) {
            this.x = x;
            this.y = y;
            this.side = side;
            this.flip = flip;
        }
    }
}