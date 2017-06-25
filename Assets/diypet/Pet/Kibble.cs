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
                c.GetComponent<PetBehavior>().FeedPet(2);
                transform.position = GameObject.Find("Pet Mouth").transform.position;

                StartCoroutine(WaitAndDestroy());
            }
        }

        IEnumerator WaitAndDestroy()
        {
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }
    }
}