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
            // Yes
            [HideInInspector]
            public StateID needID;
            [HideInInspector]
            public Transition transition;

            public float currentNeedLevel = 0;
            public float maxNeedLevel;
            public float needThreshold;
            [Range(1, 30)]
            public float minNeedGainRate;
            [Range(1, 30)]
            public float maxNeedGainRate;

            public float timeSpentInNeed;

        }
        // Just replace this with individual need classes
        //public Need[] needs;

        public Need hungryNeed;
        public Need dirtyNeed;
        public Need sleepyNeed;
        public Need lonelyNeed;
        public Need boredNeed;

        public float fallAsleepThreshold;
        [Range(1, 20)]
        public float minSleepTime;
        [Range(1, 20)]
        public float maxSleepTime;

        public float timeSpentSatisifed = 0;
        public float timeSpentScreaming = 0;

        public void SetTransition(Transition t) {
            fsm.PerformTransition(t);
        }

        public void Start() {
            MakeFSM();
            interruptedStateTransition = Transition.IsSatisfied;

            hungryNeed.needID = StateID.Hungry;
            hungryNeed.transition = Transition.IsHungry;
            dirtyNeed.needID = StateID.Dirty;
            dirtyNeed.transition = Transition.IsDirty;
            sleepyNeed.needID = StateID.Sleepy;
            sleepyNeed.transition = Transition.IsSleepy;
            lonelyNeed.needID = StateID.Lonely;
            lonelyNeed.transition = Transition.IsLonely;
            boredNeed.needID = StateID.Bored;
            boredNeed.transition = Transition.IsBored;
        }

        public void Update() {
            currentState = fsm.CurrentStateID.ToString();
            fsm.CurrentState.Reason();
            fsm.CurrentState.Act();
        }

        private void MakeFSM() {
            // Create all conditions/states
            SatisifiedState satisfied = new SatisifiedState(this);
            satisfied.AddTransition(Transition.IsHungry, StateID.Hungry);
            satisfied.AddTransition(Transition.IsDirty, StateID.Dirty);
            satisfied.AddTransition(Transition.IsSleepy, StateID.Sleepy);
            satisfied.AddTransition(Transition.IsLonely, StateID.Lonely);
            satisfied.AddTransition(Transition.IsBored, StateID.Bored);
            satisfied.AddTransition(Transition.IsScreaming, StateID.Screaming);

            HungryState hungry = new HungryState(this);
            hungry.AddTransition(Transition.IsSatisfied, StateID.Satisified);

            DirtyState dirty = new DirtyState(this);
            dirty.AddTransition(Transition.IsSatisfied, StateID.Satisified);

            SleepyState sleepy = new SleepyState(this);
            sleepy.AddTransition(Transition.IsSatisfied, StateID.Satisified);

            LonelyState lonely = new LonelyState(this);
            lonely.AddTransition(Transition.IsSatisfied, StateID.Satisified);

            BoredState bored = new BoredState(this);
            bored.AddTransition(Transition.IsSatisfied, StateID.Satisified);


            // Create immediate conditions/states
            ScreamingState freeFalling = new ScreamingState(this);
            freeFalling.AddTransition(Transition.IsSatisfied, StateID.Satisified);

            fsm = new FSMSystem();
            fsm.AddState(satisfied);
            fsm.AddState(hungry);
            fsm.AddState(dirty);
            fsm.AddState(sleepy);
            fsm.AddState(lonely);
            fsm.AddState(bored);
            fsm.AddState(freeFalling);
        }

        private void GetPreInterruptedState() {
            // Sorry, this is kind of gross

            if (fsm.CurrentStateID != StateID.Satisified) {
                switch (fsm.CurrentStateID) {
                    case StateID.Hungry:
                        interruptedStateTransition = Transition.IsHungry;
                        break;
                    case StateID.Dirty:
                        interruptedStateTransition = Transition.IsDirty;
                        break;
                    case StateID.Sleepy:
                        interruptedStateTransition = Transition.IsSleepy;
                        break;
                    case StateID.Lonely:
                        interruptedStateTransition = Transition.IsLonely;
                        break;
                    case StateID.Bored:
                        interruptedStateTransition = Transition.IsBored;
                        break;
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

        public void IncrementAllNeeds() {
            // This will take each individual need and increment it per call
            // for each need it will take the min and max and randomly pick a value
            // within that range
            IncrementNeed(hungryNeed);
            IncrementNeed(dirtyNeed);
            IncrementNeed(sleepyNeed);
            IncrementNeed(lonelyNeed);
            IncrementNeed(boredNeed);
        }

        //        public void IncrementNeed(StateID needID) {
        public void IncrementNeed(Need need) {
            // Increment the value of an individual need
            float needVal = 0;
            if (need.minNeedGainRate >= need.maxNeedGainRate) {
                needVal = need.maxNeedGainRate;
            } else {
                needVal = UnityEngine.Random.Range(need.minNeedGainRate, need.maxNeedGainRate);
            }
            need.currentNeedLevel += needVal * Time.deltaTime;
            need.currentNeedLevel = Mathf.Clamp(need.currentNeedLevel, 0, need.maxNeedLevel);
        }

        // States/Conditions and their related function calls

        public class SatisifiedState : FSMState {

            private PetBehavior behavior;

            public SatisifiedState(PetBehavior petBehavior) {
                stateID = StateID.Satisified;
                behavior = petBehavior;
            }

            public override void Reason() {

                List<Need> needToBeAddressed = new List<Need>();
                // We go through each need and see if any are over the threshold
                // and need to change states
                if (behavior.hungryNeed.currentNeedLevel >= behavior.hungryNeed.needThreshold) {
                    needToBeAddressed.Add(behavior.hungryNeed);
                }
                if (behavior.dirtyNeed.currentNeedLevel >= behavior.dirtyNeed.needThreshold) {
                    needToBeAddressed.Add(behavior.dirtyNeed);
                }
                if (behavior.sleepyNeed.currentNeedLevel >= behavior.sleepyNeed.needThreshold) {
                    needToBeAddressed.Add(behavior.sleepyNeed);
                }
                if (behavior.lonelyNeed.currentNeedLevel >= behavior.lonelyNeed.needThreshold) {
                    needToBeAddressed.Add(behavior.lonelyNeed);
                }
                if (behavior.boredNeed.currentNeedLevel >= behavior.boredNeed.needThreshold) {
                    needToBeAddressed.Add(behavior.boredNeed);
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

            public override void Act() {
                behavior.IncrementAllNeeds();
                behavior.timeSpentSatisifed += Time.deltaTime;

                // Satisfied behaviors/animations go here
            }
        }

        public class HungryState : FSMState {

            private PetBehavior behavior;

            public HungryState(PetBehavior petBehavior) {
                stateID = StateID.Hungry;
                behavior = petBehavior;
            }
            
            public override void Reason() {
                if (behavior.hungryNeed.currentNeedLevel <= 0) {
                    behavior.SetTransition(Transition.IsSatisfied);
                }
            }

            public override void Act() {
                if (behavior.parallelResourceLoss) {
                    behavior.IncrementAllNeeds();
                } else {
                    behavior.IncrementNeed(behavior.hungryNeed);
                }
                behavior.hungryNeed.timeSpentInNeed += Time.deltaTime;
                // Hungry behaviors/animations go here
            }
        }

        public void FeedPet(float feedAmount) {
            hungryNeed.currentNeedLevel -= feedAmount;
            if (hungryNeed.currentNeedLevel < 0) {
                hungryNeed.currentNeedLevel = 0;
            }
        }

        public class DirtyState : FSMState {

            private PetBehavior behavior = null;

            public DirtyState(PetBehavior petBehavior) {
                stateID = StateID.Dirty;
                behavior = petBehavior;
            }

            public override void Reason() {
                if (behavior.dirtyNeed.currentNeedLevel <= 0) {
                    behavior.SetTransition(Transition.IsSatisfied);
                }
            }

            public override void Act() {
                if (behavior.parallelResourceLoss) {
                    behavior.IncrementAllNeeds();
                } else {
                    behavior.IncrementNeed(behavior.dirtyNeed);
                }
                behavior.dirtyNeed.timeSpentInNeed += Time.deltaTime;
                // Dirty behaviors/animations go here
            }
        }

        public void CleanPet(float cleanAmount) {
            dirtyNeed.currentNeedLevel -= cleanAmount;
            if (dirtyNeed.currentNeedLevel < 0) {
                dirtyNeed.currentNeedLevel = 0;
            }
        }

        public class SleepyState : FSMState {

            private PetBehavior behavior;
            private float timeToSleep;
            private float timeSlept;

            public SleepyState(PetBehavior petBehavior) {
                stateID = StateID.Sleepy;
                behavior = petBehavior;
            }

            public override void DoBeforeEntering() {
                timeToSleep = UnityEngine.Random.Range(behavior.minSleepTime, behavior.maxSleepTime);
                timeSlept = 0;
            }

            public override void Reason() {
                if (behavior.sleepyNeed.currentNeedLevel <= 0) {
                    behavior.SetTransition(Transition.IsSatisfied);
                }
            }

            public override void Act() {
                if (behavior.parallelResourceLoss) {
                    behavior.IncrementAllNeeds();
                } else {
                    behavior.IncrementNeed(behavior.sleepyNeed);
                }

                // Sleepy/Sleep are special in the conditions/states
                // in that a single State covers both of the needs.
                // If we were to spend more time on this I would probably
                // Split them out into separate states, however due to choices
                // I made earlier this is probably the best way to approach this

                behavior.sleepyNeed.timeSpentInNeed += Time.deltaTime;
                if (behavior.sleepyNeed.currentNeedLevel >= behavior.fallAsleepThreshold) {
                    // Sleep behaviors/animations go here
                    timeSlept += Time.deltaTime;
                    if (timeSlept >= timeToSleep) {
                        behavior.WakePet(behavior.sleepyNeed.currentNeedLevel);
                    }
                } else {
                    // Sleepy behaviors/animations go here
                }
            }
        }

        public void WakePet(float wakeAmount) {
            sleepyNeed.currentNeedLevel -= wakeAmount;
            if (sleepyNeed.currentNeedLevel < 0) {
                sleepyNeed.currentNeedLevel = 0;
            }
        }

        public class LonelyState : FSMState {

            private PetBehavior behavior;

            public LonelyState(PetBehavior petBehavior) {
                stateID = StateID.Lonely;
                behavior = petBehavior;
            }

            public override void Reason() {
                if (behavior.lonelyNeed.currentNeedLevel <= 0) {
                    behavior.SetTransition(Transition.IsSatisfied);
                }
            }

            public override void Act() {
                if (behavior.parallelResourceLoss) {
                    behavior.IncrementAllNeeds();
                } else {
                    behavior.IncrementNeed(behavior.lonelyNeed);
                }
                behavior.lonelyNeed.timeSpentInNeed += Time.deltaTime;
                // Lonely behaviors/animations go here
            }
        }

        public void KeepPetCompany(float companyAmount) {
            lonelyNeed.currentNeedLevel -= companyAmount;
            if (lonelyNeed.currentNeedLevel < 0) {
                lonelyNeed.currentNeedLevel = 0;
            }
        }

        public class BoredState : FSMState {

            private PetBehavior behavior;

            public BoredState(PetBehavior petBehavior) {
                stateID = StateID.Bored;
                behavior = petBehavior;
            }

            public override void Reason() {
                if (behavior.boredNeed.currentNeedLevel <= 0) {
                    behavior.SetTransition(Transition.IsSatisfied);
                }
            }

            public override void Act() {
                if (behavior.parallelResourceLoss) {
                    behavior.IncrementAllNeeds();
                } else {
                    behavior.IncrementNeed(behavior.boredNeed);
                }
                behavior.boredNeed.timeSpentInNeed += Time.deltaTime;
                // Bored behaviors/animations go here
            }
        }

        public void EntertainPet(float entertainAmount) {
            boredNeed.currentNeedLevel -= entertainAmount;
            if (boredNeed.currentNeedLevel < 0) {
                boredNeed.currentNeedLevel = 0;
            }
        }

        // Immediate States/Conditions and related function calls

        public class ScreamingState : FSMState {

            PetBehavior behavior;

            public ScreamingState(PetBehavior petBehavior) {
                stateID = StateID.Screaming;
                behavior = petBehavior;
            }

            public override void Reason() {
                // We don't check internally for changing out of immediate states
            }

            public override void Act() {
                behavior.timeSpentScreaming += Time.deltaTime;
                // Screaming behaviors/animations go here
            }
        }

        public void StartScreaming() {
            if (fsm.CurrentStateID != StateID.Screaming) {
                GetPreInterruptedState();
                if (fsm.CurrentStateID != StateID.Satisified) {
                    fsm.PerformTransition(Transition.IsSatisfied);
                }
                fsm.PerformTransition(Transition.IsScreaming);
            }
        }

        public void EndScreaming() {
            if (fsm.CurrentStateID == StateID.Screaming) {
                ReturnToPreInterruptedState();
            }
        }
    }
}