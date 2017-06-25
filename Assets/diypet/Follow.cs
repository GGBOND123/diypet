using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet
{
    public class Follow : MonoBehaviour
    {
        public GameObject followObject;
        public Vector3 rotation;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.position = followObject.transform.position;
            transform.rotation = followObject.transform.rotation;
            transform.Rotate(rotation);
        }
    }
}