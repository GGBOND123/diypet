using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet {
    public class Clean : MonoBehaviour
    {
        void OnTriggerEnter(Collider c)
        {
            if (c.tag == "Pet")
            {
                c.GetComponent<PetBehavior>().CleanPet(2);
            }
        }
    }
}