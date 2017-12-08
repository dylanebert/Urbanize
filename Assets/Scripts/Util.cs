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
        "Jessenia"
    });

    public static List<Color> ShirtColors = new List<Color>(new Color[] {
        Palette.Alizarin,
        Palette.Carrot,
        Palette.Amethyst,
        Palette.PeterRiver
    });

    public static int WorldStateEncodingLength = 64;
    public static string[] WorldStateProperties = new string[] {
        "isLand",               //0
        "isOcean",              //1
        "isLake",               //2
        "trees",                //3
        "rocks",                //4
        "storehouse",           //5
        "occupied",             //6
        "claimed",              //7
        "innavigable"           //8
    };
    public static Dictionary<string, int> WorldStateIndices = WorldStateProperties.Select((v, i) => new { v, i }).ToDictionary(pair => pair.v, pair => pair.i);

    public static Vector2 GroundVector2(Vector3 v) {
        return new Vector2(v.x, v.z);
    }	

    public static Vector3 GroundVector3(Vector3 v) {
        return new Vector3(v.x, 0, v.z);
    }

    public static Vector3 CoordsToVector3(Vector2 coords) {
        return new Vector3(coords.x, 0, coords.y);
    }

    public static Vector3 CoordsToVector3WithSize(Vector2 coords, Vector2 size) {
        return new Vector3(coords.x + (size.x - 1) / 2f, 0, coords.y + (size.y - 1) / 2f);
    }
}
