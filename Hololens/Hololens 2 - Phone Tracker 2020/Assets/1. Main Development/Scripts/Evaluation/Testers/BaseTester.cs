using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace UOPHololens.Evaluation
{
    public abstract class BaseTester : MonoBehaviour
    {
        [NonSerialized] public ThesisEvaluator evaluator;
        public string startText;
        public int countBeforeStart;
        public string endText;
        public int countBeforeEnd;

        protected WaitForSeconds waitOne = new WaitForSeconds(1);
        private EvaluationResults results = new EvaluationResults();
        public virtual IEnumerator StartTest()
        {
            yield return null;
        }

        protected IEnumerator beginTest()
        {
            results = new EvaluationResults();
            yield return showInfo(startText, countBeforeStart);
        }
        protected IEnumerator endTest() => showInfo(endText, countBeforeEnd);
        protected IEnumerator showInfo(string info, int countdown)
        {
            var infoTmp = evaluator.basicTestInformation;
            infoTmp.enabled = true;
            infoTmp.text = info;
            for (int i = 0; i < countdown; i++)
                yield return waitOne;
            infoTmp.enabled = false;
        }

        protected virtual void StopTest()
        {

        }

        protected virtual void TestCompleted()
        {

        }
    }
}
