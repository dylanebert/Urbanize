using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : Resource {

    [HideInInspector]
    public bool claimed;

    Rigidbody rigidBody;
    GameController gameController;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Start() {
        rigidBody.AddForce(Vector3.up * 2f, ForceMode.Impulse);
    }

    public override IEnumerator PickUp(Human human) {
        if (human.state.holding != null)
            yield break;
        yield return StartCoroutine(human.MoveTo(this.transform.position));
        human.state.targetResource = null;
        human.state.holding = this;
        transform.parent = human.transform;
        transform.localPosition = new Vector3(-.1f, .35f, 0f);
        transform.localRotation = Quaternion.identity;
        GetComponent<Collider>().enabled = false;
        rigidBody.isKinematic = true;
    }

    public override IEnumerator Deposit(Human human) {
        Storehouse storehouse = human.FindNearestStorehouse();
        if (storehouse == null)
            yield break;
        yield return StartCoroutine(human.MoveTo(Vector3.MoveTowards(storehouse.front.transform.position, human.transform.position, .1f)));
        human.state.holding = null;
        storehouse.inventory.wood++;
        Destroy();
    }

    public void Destroy() {
        gameController.wood.Remove(this);
        Destroy(this.gameObject);
    }
}
