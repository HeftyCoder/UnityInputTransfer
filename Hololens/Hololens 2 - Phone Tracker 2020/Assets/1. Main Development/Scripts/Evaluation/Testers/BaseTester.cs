using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace UOPHololens.Evaluation
{
    public abstract class BaseTester : MonoBehaviour
    {
        public ThesisEvaluator evaluator;
        public string startText;
        public int countBeforeStart;
        public string endText;
        public int countBeforeEnd;

        protected WaitForSeconds waitOne = new WaitForSeconds(1);

        protected virtual IEnumerator StartTest()
        {
            yield return null;
        }
        protected virtual void StopTest()
        {

        }

        protected virtual void TestCompleted()
        {

        }
    }
}
