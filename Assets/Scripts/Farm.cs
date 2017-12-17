using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour, IWorldSelectable {

    public FarmData data;
    public FarmInfoWindow farmInfoWindowObj;
    public MeshRenderer[] grain;
    public GameObject dirtParticlesObj;
    public GameObject foodObj;

    GameController gameController;
    FarmInfoWindow farmInfoWindow;
    bool selected;

    private void Awake() {
        gameController = Util.FindGameController();
    }

    public void Initialize(FarmData farmData) {
        this.data = farmData;
        this.gameObject.name = farmData.name;
        UpdateDict();
    }

    public void UpdateDict() {
        if (!gameController.farmDict.ContainsKey(data))
            gameController.farmDict.Add(data, this);
        else
            gameController.farmDict[data] = this;
    }

    public void Step(float deltaTime) {
        if (data.yield == 1f) {
            data.isNew = false;
            return;
        }
        float deltaYield = deltaTime * .02f;
        data.yield = Mathf.Min(1f, data.yield + deltaYield);
        data.pendingYield = Mathf.Min(1f, data.pendingYield + deltaYield);
        foreach (MeshRenderer m in grain) {
            float v = data.yield / 1f;
            m.transform.localScale = new Vector3(m.transform.localScale.x, v * .1f, m.transform.localScale.z);
            m.transform.localPosition = new Vector3(m.transform.localPosition.x, v * .05f, m.transform.localPosition.z);
            if (data.yield == 1f)
                m.material.color = Palette.GrainReady;
            else
                m.material.color = Color.Lerp(Palette.GrainStart, Palette.GrainEnd, v);
        }
    }

    public IEnumerator Harvest(Human human, int row) {
        MeshRenderer grainRow = grain[row];
        Vector3 humanStartPos = new Vector3(grainRow.transform.position.x, 0, grainRow.transform.position.z -.5f);
        Vector3 humanTargetPos = new Vector3(grainRow.transform.position.x, 0, grainRow.transform.position.z + .3f);
        Vector3 startPos = grainRow.transform.localPosition;
        Vector3 targetPos = new Vector3(grainRow.transform.localPosition.x, grainRow.transform.localPosition.y, grainRow.transform.localPosition.z + .375f);
        Vector3 startScale = grainRow.transform.localScale;
        Vector3 targetScale = new Vector3(grainRow.transform.localScale.x, grainRow.transform.localScale.y, .01f);

        yield return human.MoveTo(humanStartPos);

        Quaternion startRot = human.transform.rotation;
        Quaternion tarRot = Quaternion.LookRotation((humanTargetPos - humanStartPos), Vector3.up);
        float t = 0f;
        while(t < 1f) {
            t += Time.deltaTime * 4f;
            float v = Mathf.Sin(Mathf.PI * t / 2f);
            human.transform.rotation = Quaternion.Lerp(startRot, tarRot, v);
            yield return null;
        }
        human.transform.LookAt(humanTargetPos);

        GameObject dirtParticles = Instantiate(dirtParticlesObj, human.transform);
        dirtParticles.transform.localPosition = human.transform.forward * .1f;
        human.animator.SetBool("Harvesting", true);

        t = 0f;
        while(t < 1f) {
            t = Mathf.Clamp01(t + Time.deltaTime / Human.HarvestSpeed);
            human.transform.position = Vector3.Lerp(humanStartPos, humanTargetPos, t);
            grainRow.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            grainRow.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        Destroy(dirtParticles);
        human.animator.SetBool("Harvesting", false);

        grainRow.gameObject.SetActive(false);
        grainRow.transform.localPosition = new Vector3(startPos.x, 0, startPos.z);
        grainRow.transform.localScale = new Vector3(startScale.x, .001f, startScale.z);

        Instantiate(foodObj, grainRow.transform.TransformPoint(startPos), Quaternion.identity);
    }

    public void ResetFarm() {
        foreach (MeshRenderer meshRenderer in grain)
            meshRenderer.gameObject.SetActive(true);
        data.rows = 4;
        data.pendingRows = 4;
        data.yield = 0;
        data.pendingYield = 0;
    }

    public void Dehover() {
        gameController.pointer.ShowCursorIndicator(true);
    }

    public void Deselect() {
        if (!selected)
            return;
        StartCoroutine(farmInfoWindow.CloseCoroutine());
        gameController.pointer.ShowSelectIndicator(false);
        selected = false;
    }

    public void Hover() {
        gameController.pointer.SetCursorIndicatorPosition(Util.Vector3ToCoords(transform.position), new Coords(1, 1));
    }

    public void Select() {
        if (selected)
            return;
        gameController.pointer.SetSelectIndicatorPosition(Util.Vector3ToCoords(transform.position), new Coords(1, 1));
        farmInfoWindow = Instantiate(farmInfoWindowObj, transform.position, Quaternion.identity, this.transform).GetComponent<FarmInfoWindow>();
        farmInfoWindow.Initialize(this);
        selected = true;
    }
}

[System.Serializable]
public class FarmData {
    public string name;
    public Coords coords;
    public float yield;
    public float pendingYield;
    public int rows;
    public int pendingRows;
    public bool isNew;

    public FarmData(Coords coords, string name) {
        this.name = name;
        this.coords = coords;
        this.rows = 4;
        this.pendingRows = 4;
        this.isNew = true;
    }
}
