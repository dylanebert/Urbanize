using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

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

    public GameObject dustParticleObj;
    public Indicator indicator;
    public DayNightCycle dayNightCycle;
    public GameObject woodObj;
    public Camera uiCam;

    [HideInInspector]
    public Pointer pointer;
    [HideInInspector]
    public Dictionary<Vector2, Voxel> grid = new Dictionary<Vector2, Voxel>();
    [HideInInspector]
    public List<Tree> trees = new List<Tree>();
    [HideInInspector]
    public List<Rock> rocks = new List<Rock>();
    [HideInInspector]
    public List<Building> buildings = new List<Building>();
    [HideInInspector]
    public List<Human> humans;
    [HideInInspector]
    public List<Wood> wood;
    [HideInInspector]
    public WorldWindow uniqueWorldWindow;
    [HideInInspector]
    public Indicator selectedIndicator;
    [HideInInspector]
    public bool ready;

    private void Awake() {
        pointer = GetComponent<Pointer>();
    }

    private void Start() {
        Time.timeScale = 1f;
    }

    public void ShowIndicator(GameObject target, Vector2 coords, Vector2 size) {
        indicator.target = target;
        indicator.transform.position = new Vector3(coords.x + size.x / 2, 0, coords.y + size.y / 2);
        indicator.transform.localScale = new Vector3(size.x, 1, size.y);
        indicator.gameObject.SetActive(true);
    }

    public void HideIndicator() {
        indicator.target = null;
        indicator.gameObject.SetActive(false);
    }

    public void AddStorehouse(Storehouse storehouse) {
        buildings.Add(storehouse);
        foreach (Human human in humans)
            human.state.storehouse = null;
    }

    public void AddHuman(Human human) {
        humans.Add(human);
    }
}

[System.Serializable]
public class WorldState {

}
