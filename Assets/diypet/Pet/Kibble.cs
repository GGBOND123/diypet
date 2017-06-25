using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet
{
    public class Kibble : MonoBehaviour
    {
        void OnTriggerEnter(Collider c)
        {
            if (c.tag == "Pet")
            {
                c.GetComponent<PetBehavior>().hungryNeed.currentNeedLevel = 0;
                Destroy(gameObject);
            }
        }
    }

}