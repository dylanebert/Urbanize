using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {

    public static float ExpireTime = 60f;

    protected GameController gameController;
    protected Rigidbody rigidBody;
    protected bool pickedUp;
    protected float lifeTimer;

    protected virtual void Awake() {
        gameController = Util.FindGameController();
        rigidBody = GetComponent<Rigidbody>();
        gameController.looseResources.Add(this);
    }

    private void Start() {
        rigidBody.AddForce(Vector3.up * 2f, ForceMode.Impulse);
    }

    private void Update() {
        if(!pickedUp)
            lifeTimer += Time.deltaTime;
        if(lifeTimer > ExpireTime) {
            gameController.looseResources.Remove(this);
            Destroy(this.gameObject);
        }
    }

    public virtual IEnumerator PickUp(Human human) {
        pickedUp = true;
        gameController.looseResources.Remove(this);
        yield return human.MoveTo(this.transform.position);
        transform.parent = human.transform;
        transform.localPosition = new Vector3(-.075f, .25f, 0f);
        transform.localRotation = Quaternion.identity;
        GetComponent<Collider>().enabled = false;
        rigidBody.isKinematic = true;
    }

    public virtual IEnumerator Deposit(Human human, object target) {
        throw new System.NotImplementedException();
    }
}
