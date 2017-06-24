using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet
{
    public class DissappearWhenClose : MonoBehaviour
    {
        public GameObject head;
        private bool wasClose = false;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Vector3.Distance(head.transform.position, gameObject.transform.position) < 0.75f)
            {
                wasClose = true;
            }            

            if (wasClose)
            {
                transform.Translate(Vector3.forward * 0.1f);
            }
        }
    }
}