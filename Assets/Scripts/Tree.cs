using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {

    public delegate void DropWood(Wood wood);
    public DropWood dropWood;

    public static float shakeInterval = .05f;

    [HideInInspector]
    public MeshRenderer meshRenderer;
    [HideInInspector]
    public bool claimed;

    GameController gameController;
    Voxel voxel;
    bool stop;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Initialize(Voxel voxel) {
        this.voxel = voxel;
    }

    public IEnumerator Chop(Human human, float duration) {
        ParticleSystem dust = Instantiate(gameController.dustParticleObj, this.transform).GetComponentInChildren<ParticleSystem>();
        float t = 0f;
        float t2 = 0f;
        while(t < duration - 1.5f) {
            if (stop) yield break;
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
            if (stop) yield break;
            t += Time.deltaTime;
            float v = Mathf.Pow(t, 3);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, v);
            yield return null;
        }

        while (t < 1.5f) {
            if (stop) yield break;
            t += Time.deltaTime;
            yield return null;
        }

        Wood wood = Instantiate(gameController.woodObj, transform.position + Vector3.up * .1f, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)).GetComponent<Wood>();
        human.state.lastTreeChoppedCoords = new Vector2(this.transform.position.x, this.transform.position.z);
        gameController.wood.Add(wood);       

        Destroy();
    }

    public void Destroy() {
        StopAllCoroutines();
        stop = true;
        gameController.trees.Remove(this);
        voxel.trees.Remove(this);
        Destroy(this.gameObject);
    }
}
