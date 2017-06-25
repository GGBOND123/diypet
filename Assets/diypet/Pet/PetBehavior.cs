using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace diypet
{
    public class PetBehavior : MonoBehaviour {
        private FSMSystem fsm;

        public bool runPetBehavior;

        public string currentState;

        private Transition interruptedStateTransition;

        // setting this to true will allow other needs/conditions to build while
        // the player is dealing with a present condition/need 
        public bool parallelResourceLoss;

        [Serializable]
        public class Need {
            [HideInInspector]
            public StateID needID;
            [HideInInspector]
            public Transition transition;

            public float currentNeedLevel = 0;
            public float maxNeedLevel;
            public float needThreshold;
            [Range(1, 30)]
            public float minNeedChangeRate;
            [Range(1, 30)]
            public float maxNeedChangeRate;

            public float timeSpentInNeed;

        }

        public Need hungryNeed;
        public Need dirtyNeed;
        public Need sleepyNeed;
        public Need lonelyNeed;
        public Need boredNeed;
        public Need happyNeed;

        public float fallAsleepThreshold;
        [Range(1, 20)]
        public float minSleepTime;
        [Range(1, 20)]
        public float maxSleepTime;

        private bool leaveInterruptedState = false;
        private float screamTime;
        private float currentScreamTime;

        [Range(1, 10)]
        public float minTimeScreaming;
        [Range(1, 10)]
        public float maxTimeScreaming;

        public float timeSpentSatisifed = 0;
        public float timeSpentScreaming = 0;

        [HideInInspector]
        public Animator petAnimator;


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

            petAnimator = gameObject.GetComponent<Animator>();
        }

        public void Update() {
            if (runPetBehavior) {
                currentState = fsm.CurrentStateID.ToString();
                fsm.CurrentState.Reason();
                fsm.CurrentState.Act();
                // This is a bad hack, I'm sorry
                if (leaveInterruptedState) {
                    EndScreaming();
                }
            }
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

            HappyState happy = new HappyState(this);
            happy.AddTransition(Transition.IsSatisfied, StateID.Satisified);


            // Create immediate conditions/states
            ScreamingState screaming = new ScreamingState(this);
            screaming.AddTransition(Transition.IsSatisfied, StateID.Satisified);

            fsm = new FSMSystem();
            fsm.AddState(satisfied);
            fsm.AddState(hungry);
            fsm.AddState(dirty);
            fsm.AddState(sleepy);
            fsm.AddState(lonely);
            fsm.AddState(bored);
            fsm.AddState(screaming);
            fsm.AddState(happy);
        }

        private void GetPreInterruptedState() {
            // Sorry, this is kind of gross
            // We grab the state the pet was in before transitioning to the
            // immediate state

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
                    case StateID.Happy:
                        interruptedStateTransition = Transition.IsHappy;
                        break;
                }
            }
        }

        private void ReturnToPreInterruptedState() {
            // Also gross
            // Return to the state we were in before the immediate state
            // Important to note that we do step briefly into the satisfied state
            // before returning to the state that was interrupted
            fsm.PerformTransition(Transition.IsSatisfied);
            if (interruptedStateTransition != Transition.IsSatisfied) {
                fsm.PerformTransition(interruptedStateTransition);
            }
            interruptedStateTransition = Transition.IsSatisfied;
        }

        public void IncrementAllReactiveNeeds() {
            // Takes each reactive need and increments it
            // Each need will be incremented by a random value between
            // minNeedChangeRate and maxNeedChangeRate for each need
            IncrementReactiveNeed(hungryNeed);
            IncrementReactiveNeed(dirtyNeed);
            IncrementReactiveNeed(sleepyNeed);
            IncrementReactiveNeed(lonelyNeed);
            IncrementReactiveNeed(boredNeed);
        }

        public void IncrementReactiveNeed(Need need) {
            // Increment the value of an individual reactive need
            float needVal = 0;
            if (need.minNeedChangeRate >= need.maxNeedChangeRate) {
                needVal = need.maxNeedChangeRate;
            } else {
                needVal = UnityEngine.Random.Range(need.minNeedChangeRate, need.maxNeedChangeRate);
            }
            need.currentNeedLevel += needVal * Time.deltaTime;
            need.currentNeedLevel = Mathf.Clamp(need.currentNeedLevel, 0, need.maxNeedLevel);
        }

        public void DecrementAllActiveNeeds() {
            // Takes each active need and decrements it
            // Each need will be decremented by a random value between
            // minNeedChangeRate and maxNeedChangeRate for each need
            DecrementActiveNeed(happyNeed);
        }

        public void DecrementActiveNeed(Need need) {
            // Decrement the value of an individual active need
            float needVal = 0;
            if (need.minNeedChangeRate >= need.maxNeedChangeRate) {
                needVal = need.maxNeedChangeRate;
            } else {
                needVal = UnityEngine.Random.Range(need.minNeedChangeRate, need.maxNeedChangeRate);
            }
            need.currentNeedLevel -= needVal * Time.deltaTime;
            need.currentNeedLevel = Mathf.Clamp(need.currentNeedLevel, 0, need.maxNeedLevel);
        }

        public bool ReactiveNeedsAboveThreshold() {
            bool needAboveThreshold = false;

            if (hungryNeed.currentNeedLevel >= hungryNeed.needThreshold) {
                needAboveThreshold = true;
            }
            else if (dirtyNeed.currentNeedLevel >= dirtyNeed.needThreshold) {
                needAboveThreshold = true;
            }
            else if (sleepyNeed.currentNeedLevel >= sleepyNeed.needThreshold) {
                needAboveThreshold = true;
            }
            else if (lonelyNeed.currentNeedLevel >= lonelyNeed.needThreshold) {
                needAboveThreshold = true;
            }
            else if (boredNeed.currentNeedLevel >= boredNeed.needThreshold) {
                needAboveThreshold = true;
            }
            return needAboveThreshold;
        }

        // SatisifedState is the baseline state for the pet

        public class SatisifiedState : FSMState {

            private PetBehavior behavior;
            private GameObject petGameObject;
            private Animator petAnimator;

            public SatisifiedState(PetBehavior petBehavior) {
                stateID = StateID.Satisified;
                behavior = petBehavior;
                petGameObject = behavior.gameObject;
                petAnimator = behavior.petAnimator;
            }

            public override void DoBeforeEntering() {
                
            }

            public override void DoBeforeLeaving() {
                
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
                behavior.IncrementAllReactiveNeeds();
                behavior.DecrementAllActiveNeeds();
                behavior.timeSpentSatisifed += Time.deltaTime;

                // Satisfied behaviors/animations go here
            }
        }

        // Active States/Conditions and their related function calls
        // Active states are exited for a couple of reasons:
        // 1. A reactive need has reached it's threshold and therefore needs to be moved to OR:
        // 2. The leve of the Active state falls below it's threshold and so it falls back into
        // the satisified state.
        // Active states lose their currentNeedLevel each update by minNeedChangeRate and maxNeedChangeRate

        public class HappyState : FSMState {
            private PetBehavior behavior;
            private GameObject petGameObject;
            private Animator petAnimator;

            public HappyState(PetBehavior petBehavior) {
                stateID = StateID.Happy;
                behavior = petBehavior;
                petGameObject = behavior.gameObject;
                petAnimator = behavior.petAnimator;
            }

            public override void DoBeforeEntering() {

            }

            public override void DoBeforeLeaving() {

            }

            public override void Reason() {
                
                if (behavior.ReactiveNeedsAboveThreshold()) {
                    behavior.SetTransition(Transition.IsSatisfied);
                } else if (behavior.happyNeed.needThreshold >= behavior.happyNeed.currentNeedLevel) {
                    behavior.SetTransition(Transition.IsSatisfied);
                }
            }

            public override void Act() {
                if (behavior.parallelResourceLoss) {
                    behavior.IncrementAllReactiveNeeds();
                    behavior.DecrementAllActiveNeeds();
                } else {
                    behavior.DecrementActiveNeed(behavior.happyNeed);
                }
                behavior.happyNeed.timeSpentInNeed += Time.deltaTime;

                // Happy behaviors go here
            }
        }

        public void MakePetHappy(float happyAmount) {
            happyNeed.currentNeedLevel += happyAmount;
            if (hungryNeed.currentNeedLevel > happyNeed.maxNeedLevel) {
                hungryNeed.currentNeedLevel = happyNeed.maxNeedLevel;
            }
        }

        // Reactive States/Conditions and their related function calls

        public class HungryState : FSMState {

            private PetBehavior behavior;
            private GameObject petGameObject;
            private Animator petAnimator;

            public HungryState(PetBehavior petBehavior) {
                stateID = StateID.Hungry;
                behavior = petBehavior;
                petGameObject = behavior.gameObject;
                petAnimator = behavior.petAnimator;
            }

            public override void DoBeforeEntering() {

            }

            public override void DoBeforeLeaving() {

            }

            public override void Reason() {
                if (behavior.hungryNeed.currentNeedLevel <= 0) {
                    behavior.SetTransition(Transition.IsSatisfied);
                }
            }

            public override void Act() {
                if (behavior.parallelResourceLoss) {
                    behavior.IncrementAllReactiveNeeds();
                    behavior.DecrementAllActiveNeeds();
                } else {
                    behavior.IncrementReactiveNeed(behavior.hungryNeed);
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

            private PetBehavior behavior;
            private GameObject petGameObject;
            private Animator petAnimator;

            public DirtyState(PetBehavior petBehavior) {
                stateID = StateID.Dirty;
                behavior = petBehavior;
                petGameObject = behavior.gameObject;
                petAnimator = behavior.petAnimator;
            }

            public override void DoBeforeEntering() {

            }

            public override void DoBeforeLeaving() {

            }

            public override void Reason() {
                if (behavior.dirtyNeed.currentNeedLevel <= 0) {
                    behavior.SetTransition(Transition.IsSatisfied);
                }
            }

            public override void Act() {
                if (behavior.parallelResourceLoss) {
                    behavior.IncrementAllReactiveNeeds();
                    behavior.DecrementAllActiveNeeds();
                } else {
                    behavior.IncrementReactiveNeed(behavior.dirtyNeed);
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
            private GameObject petGameObject;
            private Animator petAnimator;
            private float timeToSleep;
            private float timeSlept;

            public SleepyState(PetBehavior petBehavior) {
                stateID = StateID.Sleepy;
                behavior = petBehavior;
                petGameObject = behavior.gameObject;
                petAnimator = behavior.petAnimator;
            }

            public override void DoBeforeEntering() {
                if (behavior.minSleepTime >= behavior.maxSleepTime) {
                    timeToSleep = behavior.maxSleepTime;
                } else {
                    timeToSleep = UnityEngine.Random.Range(behavior.minSleepTime, behavior.maxSleepTime);
                }
                timeSlept = 0;
            }

            public override void DoBeforeLeaving() {

            }

            public override void Reason() {
                if (behavior.sleepyNeed.currentNeedLevel <= 0) {
                    behavior.SetTransition(Transition.IsSatisfied);
                }
            }

            public override void Act() {
                if (behavior.parallelResourceLoss) {
                    behavior.IncrementAllReactiveNeeds();
                    behavior.DecrementAllActiveNeeds();
                } else {
                    behavior.IncrementReactiveNeed(behavior.sleepyNeed);
                }
                behavior.sleepyNeed.timeSpentInNeed += Time.deltaTime;

                // Sleepy and sleep behaviors/animations go here
                // If needed timeToSleep and timeSlept are available for use if needed

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
            private GameObject petGameObject;
            private Animator petAnimator;

            public LonelyState(PetBehavior petBehavior) {
                stateID = StateID.Lonely;
                behavior = petBehavior;
                petGameObject = behavior.gameObject;
                petAnimator = behavior.petAnimator;
            }

            public override void DoBeforeEntering() {

            }

            public override void DoBeforeLeaving() {

            }

            public override void Reason() {
                if (behavior.lonelyNeed.currentNeedLevel <= 0) {
                    behavior.SetTransition(Transition.IsSatisfied);
                }
            }

            public override void Act() {
                if (behavior.parallelResourceLoss) {
                    behavior.IncrementAllReactiveNeeds();
                    behavior.DecrementAllActiveNeeds();
                } else {
                    behavior.IncrementReactiveNeed(behavior.lonelyNeed);
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
            private GameObject petGameObject;
            private Animator petAnimator;

            public BoredState(PetBehavior petBehavior) {
                stateID = StateID.Bored;
                behavior = petBehavior;
                petGameObject = behavior.gameObject;
                petAnimator = behavior.petAnimator;
            }

            public override void DoBeforeEntering() {

            }

            public override void DoBeforeLeaving() {

            }

            public override void Reason() {
                if (behavior.boredNeed.currentNeedLevel <= 0) {
                    behavior.SetTransition(Transition.IsSatisfied);
                }
            }

            public override void Act() {
                if (behavior.parallelResourceLoss) {
                    behavior.IncrementAllReactiveNeeds();
                    behavior.DecrementAllActiveNeeds();
                } else {
                    behavior.IncrementReactiveNeed(behavior.boredNeed);
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
            private GameObject petGameObject;
            private Animator petAnimator;

            public ScreamingState(PetBehavior petBehavior) {
                stateID = StateID.Screaming;
                behavior = petBehavior;
                petGameObject = behavior.gameObject;
                petAnimator = behavior.petAnimator;
            }

            public override void DoBeforeEntering() {

            }

            public override void DoBeforeLeaving() {

            }

            public override void Reason() {
                // We don't check internally for changing out of immediate states
            }

            public override void Act() {
                if (behavior.parallelResourceLoss) {
                    behavior.IncrementAllReactiveNeeds();
                    behavior.DecrementAllActiveNeeds();
                }
                behavior.timeSpentScreaming += Time.deltaTime;
                // Screaming behaviors/animations go here
            }

        }

        // Terrible hack below. I'm sorry, it was so nice before
        public void StartScreaming() {
            leaveInterruptedState = false;
            if (minTimeScreaming >= maxTimeScreaming) {
                screamTime = maxTimeScreaming;
            } else {
                screamTime = UnityEngine.Random.Range(minTimeScreaming, maxTimeScreaming);
            }
            currentScreamTime = 0;
            if (fsm.CurrentStateID != StateID.Screaming) {
                GetPreInterruptedState();
                if (fsm.CurrentStateID != StateID.Satisified) {
                    fsm.PerformTransition(Transition.IsSatisfied);
                }
                fsm.PerformTransition(Transition.IsScreaming);
            }
        }

        public void EndScreaming() {
            leaveInterruptedState = true;
            if (fsm.CurrentStateID == StateID.Screaming) {
                if (currentScreamTime >= screamTime) {
                    leaveInterruptedState = false;
                    ReturnToPreInterruptedState();
                } else {
                    currentScreamTime += Time.deltaTime;
                }
            }
        }
    }
}