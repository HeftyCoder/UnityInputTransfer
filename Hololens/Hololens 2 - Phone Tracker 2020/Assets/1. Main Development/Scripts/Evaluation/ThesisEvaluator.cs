using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

namespace UOPHololens.Evaluation
{
    public class ThesisEvaluator : MonoBehaviour
    {
        public enum State { Idle, Evaluating }

        internal SimpleFirebaseClient client;
        internal TMP_Text targetsLeftCounter;
        internal TMP_Text basicTestInformation;
        internal DirectionalIndicator targetIndicator;
        internal SelectableTargetsProvider targetsProvider;

        State state = State.Idle;

        public State CurrentState => state;

        public void PlayDemo()
        {
            if (Protect())
                return;

        }

        public void Stop()
        {

        }

        private bool Protect() => state != State.Idle || !enabled;
    }
}

