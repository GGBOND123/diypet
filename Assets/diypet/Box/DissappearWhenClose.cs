using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet
{
    public class DissappearWhenClose : MonoBehaviour
    {
        public GameObject head;
        private bool wasClose = false;
        public TimeManager timeManager;

		public bool onAtStart = false;

        // Use this for initialization
        void Start()
        {
			if (onAtStart) {
				wasClose = true;
				timeManager.TurnOn ();
			}
        }

        // Update is called once per frame
        void Update()
        {
            if (Vector3.Distance(head.transform.position, gameObject.transform.position) < 0.75f)
            {
                if (transform.position != Vector3.zero)
                {
                    wasClose = true;
                    timeManager.TurnOn();
                }
            }            

            if (wasClose)
            {
                transform.Translate(Vector3.forward * 0.1f);
            }
        }
    }
}