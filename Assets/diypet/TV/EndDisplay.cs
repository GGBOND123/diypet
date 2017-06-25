using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet
{
    public class EndDisplay : MonoBehaviour
    {

        public TextMesh text;

        public int levelOfRearing = 0;
        public List<string> personalityDescs;
        public PetBehavior pet;

        private bool gameOver = false;

        // Use this for initialization
        void Start()
        {
        //    SetResult();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void SetResult( )
        {
            float timeSatisfied = pet.timeSpentSatisifed;
            float maxTimeSatisfied = 100;
            float minTimeSatisfied = 20;
            float increments = (maxTimeSatisfied - minTimeSatisfied) / (float)personalityDescs.Count;
            levelOfRearing = (int)( (timeSatisfied - minTimeSatisfied) / increments);
            levelOfRearing = Mathf.Max(levelOfRearing, personalityDescs.Count - 1);

            text.text = "YOU MADE YOUR PET:\n\n" + personalityDescs[levelOfRearing];
        }


    }
}