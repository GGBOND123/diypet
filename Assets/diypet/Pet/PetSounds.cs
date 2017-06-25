using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet {
    public class PetSounds : MonoBehaviour {

        public AudioClip screamingClip;
        public AudioSource audioSource;
        public PetBehavior petBehavior;
        public string state = "";

	    // Use this for initialization
	    void Start() {

        }

        // Update is called once per frame
        void Update() {
            if (petBehavior.currentState == "Screaming" && state != "Screaming")
            {
                state = "Screaming";
                audioSource.clip = screamingClip;
                audioSource.Play();
            } else if (petBehavior.currentState != "Screaming" && state == "Screaming")
            {
                state = petBehavior.currentState;
                audioSource.Stop();
            }
        }

        
    }
}
