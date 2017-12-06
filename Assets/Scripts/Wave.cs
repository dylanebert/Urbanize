using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

    public float speed = .1f;

    private IEnumerator Start() {
        Vector3 target = transform.position + Vector3.right * .45f;
        transform.Translate(Vector3.left * 2.45f);
        while(transform.position != target) {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
            yield return null;
        }
        foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
            particle.Stop();
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }
}
