using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet {
    public class Timer : MonoBehaviour {

        private bool started = false;
        public float runningTime;

	    // Use this for initialization
	    void Start () {
		
	    }
	
	    // Update is called once per frame
	    void Update () {
		if (started)
            {
                runningTime += Time.deltaTime;
            }
	    }

        public void TurnOn()
        {
            started = true;
        }
    }
}