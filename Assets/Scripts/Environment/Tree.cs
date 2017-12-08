using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {

    public delegate void DropWood(Wood wood);
    public DropWood dropWood;

    public static float shakeInterval = .05f;

    [HideInInspector]
    public TreeData treeData;
    [HideInInspector]
    public MeshRenderer meshRenderer;
    [HideInInspector]
    public bool claimed;

    Vector2 coords;
    GameController gameController;
    bool chopping;
    bool stop;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Initialize(int type) {
        coords = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.z));
        treeData = new TreeData(Util.GroundVector2(transform.position), type);
        gameController.treeDict.Add(treeData, this);
        gameController.world.treeData.Add(treeData);
        gameController.world.SetProperty(coords, "occupied", true);
        gameController.world.SetProperty(coords, "trees", true);
    }

    public IEnumerator Chop(Human human) {
        chopping = true;
        float duration = human.state.chopSpeed;
        ParticleSystem dust = Instantiate(gameController.dustParticleObj, this.transform).GetComponentInChildren<ParticleSystem>();
        float t = 0f;
        float t2 = 0f;
        while(t < duration - 1.5f) {
            if (stop) {
                Destroy(this.gameObject);
                yield break;
            }
            t += Time.deltaTime;
            t2 += Time.deltaTime;
            if(t2 > shakeInterval) {
                t2 = 0f;
                transform.rotation = Quaternion.Euler(Random.Range(-2f, 2f), transform.rotation.eulerAngles.y, Random.Range(-2f, 2f));
            }
            yield return null;
        }
        dust.Stop();

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.Euler(85f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        t = 0f;
        while(t < 1f) {
            if (stop) {
                Destroy(this.gameObject);
                yield break;
            }
            t += Time.deltaTime;
            float v = Mathf.Pow(t, 3);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, v);
            yield return null;
        }

        while (t < 1.5f) {
            if (stop) {
                Destroy(this.gameObject);
                yield break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        Wood wood = Instantiate(gameController.woodObj, transform.position + Vector3.up * .1f, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)).GetComponent<Wood>();
        human.state.lastTreeChoppedCoords = new Vector2(this.transform.position.x, this.transform.position.z);
        gameController.wood.Add(wood);

        chopping = false;
        Destroy();
    }

    public void Destroy() {
        gameController.world.treeData.Remove(treeData);
        gameController.treeDict.Remove(treeData);
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;
        if(Physics.OverlapBox(Util.CoordsToVector3(coords), Vector3.one * .5f, Quaternion.identity, LayerMask.GetMask("Tree")).Length == 0) {
            gameController.world.SetProperty((int)coords.x, (int)coords.y, "occupied", false);
            gameController.world.SetProperty((int)coords.x, (int)coords.y, "trees", false);
        }
        stop = true;
        if (!chopping)
            Destroy(this.gameObject);
    }
}

[System.Serializable]
public class TreeData {
    public Vector2 pos;
    public int type;

    public TreeData(Vector2 pos, int type) {
        this.pos = pos;
        this.type = type;
    }
}
