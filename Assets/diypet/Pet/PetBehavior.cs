using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet
{
    public class PetBehavior : MonoBehaviour {
        private FSMSystem fsm;

        public string currentState;

        private Transition interruptedStateTransition;

        // setting this to true will allow other needs/conditions to build while
        // the player is dealing with a present condition/need 
        public bool parallelResourceLoss;

        [Serializable]
        public class Need {
            // Do we want needs to have a maximum value that they can't build past?
            public StateID needID;
            public Transition transition;

            public float currentNeedLevel = 0;
            public float hasNeedThreshold;
            [Range(1, 30)]
            public float minNeedGainRate;
            [Range(1, 30)]
            public float maxNeedGainRate;

        }

        public Need[] needs;

        // This direct transition call might not be necessary, maybe just add values for all
        // of the conditions?
        public void SetTransition(Transition t) {
            fsm.PerformTransition(t);
        }

        public void Start() {
            MakeFSM();
            interruptedStateTransition = Transition.IsSatisfied;
        }

        public void Update() {
            currentState = fsm.CurrentStateID.ToString();
            fsm.CurrentState.Reason(gameObject);
            fsm.CurrentState.Act(gameObject);
        }

        private void MakeFSM() {
            SatisifiedState satisfied = new SatisifiedState();
            satisfied.AddTransition(Transition.IsHungry, StateID.Hungry);
            satisfied.AddTransition(Transition.IsFreeFalling, StateID.FreeFalling);

            HungryState hungry = new HungryState();
            hungry.AddTransition(Transition.IsSatisfied, StateID.Satisified);

            FreeFallingState freeFalling = new FreeFallingState();
            freeFalling.AddTransition(Transition.IsSatisfied, StateID.Satisified);

            fsm = new FSMSystem();
            fsm.AddState(satisfied);
            fsm.AddState(hungry);
            fsm.AddState(freeFalling);
        }

        private void GetPreInterruptedState() {
            // Sorry, this is kind of gross
            if (fsm.CurrentStateID != StateID.Satisified) {
                for (int i = 0; i < needs.Length; i++) {
                    if (fsm.CurrentStateID == needs[i].needID) {
                        interruptedStateTransition = needs[i].transition;
                        break;
                    }
                }
            }
        }

        private void ReturnToPreInterruptedState() {
            fsm.PerformTransition(Transition.IsSatisfied);
            if (interruptedStateTransition != Transition.IsSatisfied) {
                fsm.PerformTransition(interruptedStateTransition);
            }
            interruptedStateTransition = Transition.IsSatisfied;
        }

        public void StartFreeFalling() {
            if (fsm.CurrentStateID != StateID.FreeFalling) {
                GetPreInterruptedState();
                if (fsm.CurrentStateID != StateID.Satisified) {
                    fsm.PerformTransition(Transition.IsSatisfied);
                }
                fsm.PerformTransition(Transition.IsFreeFalling);
            }
        }

        public void EndFreeFalling() {
            if (fsm.CurrentStateID == StateID.FreeFalling) {
                ReturnToPreInterruptedState();
            }
        }

        public void IncrementAllNeeds() {
            // This will take each individual need and increment it per call
            // for each need it will take the min and max and randomly pick a value
            // within that range
            StateID[] enumValues = (StateID[])Enum.GetValues(typeof(StateID));
            for (int i = 0; i < enumValues.Length; i++) {
                if (enumValues[i] != StateID.Satisified) {
                    IncrementNeed(enumValues[i]);
                }
            }
        }

        public void IncrementNeed(StateID needID) {
            // Increment the value of an individual need
            for (int i = 0; i < needs.Length; i++) {
                if (needs[i].needID == needID) {
                    float needVal = 0;

                    if (needs[i].minNeedGainRate >= needs[i].maxNeedGainRate) {
                        needVal = needs[i].maxNeedGainRate;
                    } else {
                        needVal = UnityEngine.Random.Range(needs[i].minNeedGainRate, needs[i].maxNeedGainRate);
                    }
                    needs[i].currentNeedLevel += needVal * Time.deltaTime;
                    break;
                }
            }

        }

        public void FeedPet(int feedAmount) {
            Need foodNeed = null;
            for (int i = 0; i < needs.Length; i++) {
                if (needs[i].needID == StateID.Hungry) {
                    foodNeed = needs[i];
                    break;
                }
            }
            foodNeed.currentNeedLevel -= feedAmount;
            if (foodNeed.currentNeedLevel < 0) {
                foodNeed.currentNeedLevel = 0;
            }
        }

        public class SatisifiedState : FSMState {
            public SatisifiedState() {
                stateID = StateID.Satisified;
            }

            private PetBehavior behavior = null;

            public override void Reason(GameObject pet) {
                if (behavior == null) {
                    behavior = pet.GetComponent<PetBehavior>();
                }
                List<Need> needToBeAddressed = new List<Need>();
                // We go through each need and see if any are over the threshold for needing action
                for (int i = 0; i < behavior.needs.Length; i++) {
                    Need need = behavior.needs[i];
                    if (need.currentNeedLevel >= need.hasNeedThreshold) {
                        needToBeAddressed.Add(need);
                    }
                }

                // If any are over the threshold for action we choose one state over
                // threshold to move to randomly
                if (needToBeAddressed.Count == 1) {
                    behavior.SetTransition(needToBeAddressed[0].transition);
                } else if (needToBeAddressed.Count > 1) {
                    int needIdx = UnityEngine.Random.Range(0, needToBeAddressed.Count);
                    behavior.SetTransition(needToBeAddressed[needIdx].transition);
                }
            }

            public override void Act(GameObject pet) {
                // We can add some sort of satisifed interaction here, purring etc?
                if (behavior == null) {
                    behavior = pet.GetComponent<PetBehavior>();
                }
                behavior.IncrementAllNeeds();
            }
        }

        public class HungryState : FSMState {
            public HungryState() {
                stateID = StateID.Hungry;
            }

            private PetBehavior behavior = null;

            public override void Reason(GameObject pet) {
                // If immediate behavior is set move to that
                // but store old state to switch back to after
                // out of immediate state
                if (behavior == null) {
                    behavior = pet.GetComponent<PetBehavior>();
                }
                Need hungerNeed = null;
                for (int i = 0; i < behavior.needs.Length; i++) {
                    if (behavior.needs[i].needID == stateID) {
                        hungerNeed = behavior.needs[i];
                        break;
                    }
                }
                if (hungerNeed.currentNeedLevel <= 0) {
                    behavior.SetTransition(Transition.IsSatisfied);
                }
            }

            public override void Act(GameObject pet) {
                if (behavior == null) {
                    behavior = pet.GetComponent<PetBehavior>();
                }

                if (behavior.parallelResourceLoss) {
                    behavior.IncrementAllNeeds();
                } else {
                    behavior.IncrementNeed(stateID);
                }

                // Hungry behaviors would go here
            }
        }

        public class FreeFallingState : FSMState {
            public FreeFallingState() {
                stateID = StateID.FreeFalling;
            }

            private PetBehavior behavior = null;

            public override void Reason(GameObject pet) {
                if (behavior == null) {
                    behavior = pet.GetComponent<PetBehavior>();
                }
                // We don't check internally for changing out of immediate states
            }

            public override void Act(GameObject pet) {
                if (behavior == null) {
                    behavior = pet.GetComponent<PetBehavior>();
                }
                // Behaviors here
            }
        }
    }
}