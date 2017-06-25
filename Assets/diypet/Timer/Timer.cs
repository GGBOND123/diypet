using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet {
    public class Timer : MonoBehaviour {

        private bool started = true;
        public float runningTime;
        public GameObject bird;
        public GameObject flood;

        private bool bird_spawn = false;
        private bool flood_spawn = false;

	    // Use this for initialization
	    void Start () {
		
	    }
	
	    // Update is called once per frame
	    void Update () {
		if (started)
            {
                runningTime += Time.deltaTime;
            }

            if (runningTime >= 120 && !this.bird_spawn)
            {
                this.bird.GetComponent<BirdController>().enabled = true;
            }

            if (runningTime >= 200 && !this.flood_spawn)
            {
                RaiseFlood();    
            }
        }

        public void TurnOn()
        {
            started = true;
        }

        public void RaiseFlood()
        {
            this.flood.transform.position = new Vector3(this.flood.transform.position.x, this.flood.transform.position.y + 0.25f, this.flood.transform.position.z);

            if (this.flood.transform.position.y >= -33f)
            {
                this.flood_spawn = true;
            } 
        }
    }
}