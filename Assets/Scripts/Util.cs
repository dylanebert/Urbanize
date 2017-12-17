using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Util {

    public static List<string> Names = new List<string>(new string[] {
        "Cara",
        "Luana",
        "Geneva",
        "Katherine",
        "Jene",
        "Colette",
        "Margo",
        "Joanne",
        "Goldie",
        "Bonny",
        "Francina",
        "Angelo",
        "Dani",
        "Kacey",
        "Khadijah",
        "Verona",
        "Karry",
        "Kendrick",
        "Leisa",
        "Sasha",
        "Kirby",
        "Leonardo",
        "Hazel",
        "Franchesca",
        "Vickie",
        "Margarett",
        "Esteban",
        "Harry",
        "Cassondra",
        "Willard",
        "Rina",
        "Gladys",
        "Era",
        "Akilah",
        "Fawn",
        "Dina",
        "Randal",
        "Gilda",
        "Hank",
        "Wes",
        "Alyce",
        "Ai",
        "Sherie",
        "Candyce",
        "Cordell",
        "Tanya",
        "Fran",
        "Debora",
        "Jenae",
        "Jessenia",
        "Vanya",
        "Dylan"
    });

    public static List<Color> ShirtColors = new List<Color>(new Color[] {
        Palette.Alizarin,
        Palette.Carrot,
        Palette.Amethyst,
        Palette.PeterRiver
    });

    public static Coords Vector3ToCoords(Vector3 v) {
        return new Coords(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.z));
    }	

    public static Vector3 CoordsToVector3(Coords coords) {
        return new Vector3(coords.x, 0, coords.y);
    }

    public static Vector3 CoordsToVector3WithSize(Coords coords, Coords size) {
        return new Vector3(coords.x + (size.x - 1) / 2f, 0, coords.y + (size.y - 1) / 2f);
    }

    public static GameController FindGameController() {
        return GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public static List<Coords> GetAdjacent(Coords coords, bool corners) {
        int x = coords.x;
        int y = coords.y;
        List<Coords> adjacent = new List<Coords>();
        if (x > 0) {
            adjacent.Add(new Coords(x - 1, y));
            if (corners && y > 0) {
                adjacent.Add(new Coords(x - 1, y - 1));
            }
            if (corners && y < TerrainGenerator.size - 1) {
                adjacent.Add(new Coords(x - 1, y + 1));
            }
        }
        if (x < TerrainGenerator.size - 1) {
            adjacent.Add(new Coords(x + 1, y));
            if (corners && y > 0) {
                adjacent.Add(new Coords(x + 1, y - 1));
            }
            if (corners && y < TerrainGenerator.size - 1) {
                adjacent.Add(new Coords(x + 1, y + 1));
            }
        }
        if (y > 0) {
            adjacent.Add(new Coords(x, y - 1));
        }
        if (y < TerrainGenerator.size - 1) {
            adjacent.Add(new Coords(x, y + 1));
        }
        return adjacent;
    }

    public static List<VoxelData> CoordsToVoxels(VoxelData[,] voxels, List<Coords> coords) {
        List<VoxelData> voxelData = new List<VoxelData>();
        foreach (Coords c in coords)
            voxelData.Add(voxels[c.x, c.y]);
        return voxelData;
    }

    public static bool AdjacentProperty(List<VoxelData> adjacent, string property) {
        foreach (VoxelData adj in adjacent) {
            if ((bool)typeof(VoxelData).GetField(property).GetValue(adj))
                return true;
        }
        return false;
    }

    public static int GetMaxYieldFarms(List<FarmData> farmData) {
        int i = 0;
        foreach (FarmData farm in farmData)
            if (farm.pendingYield == 1f)
                i++;
        return i;
    }

    public static T Copy<T>(T source) {
        if (!typeof(T).IsSerializable)
            throw new ArgumentException("Type must be serialiable", "source");

        if (object.ReferenceEquals(source, null))
            return default(T);

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using(stream) {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}

[Serializable]
public class Coords {
    public int x;
    public int y;

    public Coords(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static Coords Invalid {
        get {
            return new Coords(-1, -1);
        }
    }

    public override string ToString() {
        return x + ", " + y;
    }

    public override bool Equals(object obj) {
        if(obj is Coords) {
            return (((Coords)obj).x == this.x && ((Coords)obj).y == this.y);
        }
        return base.Equals(obj);
    }
}

[Serializable]
public class SerializableColor {
    public float r;
    public float g;
    public float b;

    public SerializableColor(float r, float g, float b) {
        this.r = r;
        this.g = g;
        this.b = b;
    }

    public static SerializableColor FromColor(Color color) {
        return new SerializableColor(color.r, color.g, color.b);
    }

    public Color ToColor() {
        return new Color(r, g, b);
    }
}