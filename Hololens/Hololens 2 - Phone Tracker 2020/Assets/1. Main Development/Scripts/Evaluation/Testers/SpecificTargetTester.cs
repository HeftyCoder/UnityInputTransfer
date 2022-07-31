using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace UOPHololens.Evaluation
{
    public class SpecificTargetTester : BaseTester
    {
        public int targetCount = 15;
        private int currentCount;
        public override IEnumerator StartTest()
        {
            currentCount = targetCount;
            var targetsCounter = evaluator.gameUI.TargetsCounter;
            targetsCounter.text = currentCount.ToString();
            var timeCounter = evaluator.gameUI.TimeCounter;
            float fullTime = 0;
            yield return beginTest();
            
            targetsProvider.PickNextTarget();
            while (currentCount > 0)
            {
                var delta = Time.deltaTime;
                fullTime += delta;
                results.currentTime += delta;
                timeCounter.text = fullTime.ToString();
                yield return null;
            }

            yield return endTest();
        }
        protected override void onClick(SelectableTarget target)
        {
            base.onClick(target);
            currentCount--;
            evaluator.gameUI.TargetsCounter.text = currentCount.ToString();
        }
    }
}