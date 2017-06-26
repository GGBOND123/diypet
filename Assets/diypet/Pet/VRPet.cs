using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet
{
    public class VRPet : MonoBehaviour
    {
        Vector3 velocity = Vector3.zero;
        Vector3 oldPosition = Vector3.zero;

        public PetBehavior petBehavior;
        public GameObject food;
		public GameObject bucket;
		public GameObject head;

        public float screamTime = 0;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            velocity = (transform.position - oldPosition) / Time.deltaTime;

            if (velocity.magnitude  > 3f)
            {
                petBehavior.StartScreaming();
                screamTime = 0.5f;
            } 

            if (velocity.magnitude < 3)
            {
                screamTime -= Time.deltaTime;
                if (screamTime < 0)
                {
                    petBehavior.EndScreaming();
                }

            }

            if (Vector3.Distance(food.transform.position, transform.position) < 0.5f)
            {
                petBehavior.FeedPet(1);
            }

			if (Vector3.Distance (head.transform.position, transform.position) < 0.5f) {
				petBehavior.KeepPetCompany (1);
			}

			if (Vector3.Distance (bucket.transform.position, transform.position) < 0.5f) {
				petBehavior.CleanPet (1);
			}

            if (velocity.magnitude > 0.3f)
            {
                petBehavior.EntertainPet(1);
            }

            if (velocity.magnitude > 0.3f)
            {
                petBehavior.WakePet(1);
            }


            oldPosition = transform.position;
        }
    }
}