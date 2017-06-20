using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoRagDoll : MonoBehaviour {

    private Vector3 pos_;
    private float timer = 0f;
    public List<GameObject> ragdolls;

    void Update() {
        if (timer >= 1f)
        {
            this.pos_ = transform.position;
            this.timer = 0f;
        } else
        {
            if (transform.position != this.pos_)
            {
                foreach (GameObject g in ragdolls)
                {
                    g.GetComponent<Animator>().enabled = false;
                }
            } else
            {
                foreach (GameObject g in ragdolls)
                {
                    g.GetComponent<Animator>().enabled = true;
                }
            }
            this.timer += Time.deltaTime;
        }	
	}
}
