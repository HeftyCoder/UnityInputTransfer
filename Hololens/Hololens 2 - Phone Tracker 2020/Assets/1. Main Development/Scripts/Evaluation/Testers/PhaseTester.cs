using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace UOPHololens.Evaluation { 
    public class PhaseTester : BaseTester
    {
        public enum PhasePicker { NativeFirst, PhoneFirst, Random}
        [SerializeField] TestPhaseHelper nativePhase;
        [SerializeField] TestPhaseHelper phonePhase;
        [SerializeField] PhasePicker phasePicker;

        [SerializeField] TestPhaseHelper startPhase;
        [SerializeField] TestPhaseHelper middlePhase;
        [SerializeField] TestPhaseHelper endPhase;
        TestPhaseHelper currentPhase;

        public TestPhaseHelper StartPhase => startPhase;
        public TestPhaseHelper MiddlePhase => middlePhase;
        public TestPhaseHelper EndPhase => endPhase;
        public TestPhaseHelper GetFirstPhase(EvaluationTest evTest)
        {
            var player = evaluator.player;
            results = new EvaluationResults();

            switch (phasePicker)
            {
                case PhasePicker.PhoneFirst:
                    currentPhase = phonePhase;
                    break;
                case PhasePicker.NativeFirst:
                    currentPhase = nativePhase;
                    break;
                case PhasePicker.Random:
                    currentPhase = UnityEngine.Random.Range(0, 2) == 0 ? nativePhase : phonePhase;
                    break;
            }

            if (currentPhase == nativePhase)
                evTest.nativeTest.Add(results);
            else
                evTest.phoneTest.Add(results);
            return currentPhase;
        }
        public TestPhaseHelper GetSecondPhase(EvaluationTest evTest)
        {
            results = new EvaluationResults();
            if (currentPhase == nativePhase)
            {
                currentPhase = phonePhase;
                evTest.phoneTest.Add(results);
            }
            else
            {
                currentPhase = nativePhase;
                evTest.nativeTest.Add(results);
            }

            return currentPhase;
        }

        public override void Stop()
        {
            nativePhase?.Close();
            phonePhase?.Close();
            startPhase?.Close();
            middlePhase?.Close();
            endPhase?.Close();
        }

    }
}
