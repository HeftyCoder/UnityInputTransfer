using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace UOPHololens.Evaluation
{
    public class DemoTester : BaseTester
    {
        [SerializeField] bool testing = false;
        public string startText;
        public int countBeforeStart;
        public string endText;
        public int countBeforeEnd;
        public override IEnumerator StartTest()
        {
            testing = true;
            float allTime = 0;
            evaluator.gameUI.TargetsCounter.text = "0";
            var timeTmp = evaluator.gameUI.TimeCounter;
            timeTmp.text = "0";
            results = new EvaluationResults();

            beginTest();
            yield return showInfo(startText, countBeforeStart);
            targetsProvider.PickNextTarget();
            while (testing)
            {
                var delta = Time.deltaTime;
                results.currentTime += delta;
                allTime += delta;
                timeTmp.text = allTime.ToString();
                yield return null;
            }

            endTest();
            yield return showInfo(endText, countBeforeEnd);
        }

        public override void StopTest() => testing = false;

        protected override void onClick(SelectableTarget target)
        {
            base.onClick(target);
            evaluator.gameUI.TargetsCounter.text = results.selectedCount.ToString();
        }
    }
}