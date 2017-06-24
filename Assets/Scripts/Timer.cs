using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    public float timer = 0f;
    private float cutoff = 1f;

	void Update () {
        this.timer += Time.deltaTime;
        if (this.timer >= 1f && this.cutoff > .09f)
        {
            //this.cutoff -= 0.0033f;
            this.cutoff -= 0.05f;
            GetComponent<Renderer>().material.SetFloat("_Cutoff", this.cutoff);
            this.timer = 0f;
        }
    }
}
