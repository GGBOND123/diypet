using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet
{
    public class DissappearWhenClose : MonoBehaviour
    {
        public GameObject head;
        private bool wasClose = false;
        public Timer timer;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Vector3.Distance(head.transform.position, gameObject.transform.position) < 0.75f)
            {
                if (transform.position != Vector3.zero)
                {
                    wasClose = true;
                    timer.TurnOn();
                }
            }            

            if (wasClose)
            {
                transform.Translate(Vector3.forward * 0.1f);
            }
        }
    }
}