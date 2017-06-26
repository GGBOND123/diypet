using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet {
    public class TimeManager : MonoBehaviour {

        private bool started = true;
        public float runningTime;
		public float lengthOfGame;
        public GameObject bird;
        public GameObject flood;
		public EndDisplay endDisplay;
		public Timer timer;

        private bool bird_spawn = false;
        private bool flood_raise_done = false;
		private bool gameOver = false;

	    // Use this for initialization
	    void Start () {
		
	    }
	
	    // Update is called once per frame
	    void Update () {
		if (started)
            {
                runningTime += Time.deltaTime;
            }

            if (runningTime >= 60 && !bird_spawn)
            {
                this.bird.GetComponent<BirdController>().enabled = true;
				bird_spawn = true;
            }

            if (runningTime >= 90 && !flood_raise_done)
            {
                RaiseFlood();    
            }

			if (runningTime > lengthOfGame && !gameOver) {
				endDisplay.SetResult ();
				gameOver = true;
			}
        }

        public void TurnOn()
        {
            started = true;
			timer.on = true;
        }

        public void RaiseFlood()
        {
            this.flood.transform.position = new Vector3(this.flood.transform.position.x, this.flood.transform.position.y + 0.25f, this.flood.transform.position.z);

            if (this.flood.transform.position.y >= -33f)
            {
                this.flood_raise_done = true;
            } 
        }
    }
}