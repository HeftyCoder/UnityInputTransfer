using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

namespace UOPHololens.Evaluation
{
    public class DemoTester : BaseTester
    {
        [SerializeField] bool testing = false;

        private void Awake()
        {

        }
        public override IEnumerator StartTest()
        {
            testing = true;
            float allTime = 0;
            evaluator.gameUI.Open(0, 0);
            var timeTmp = evaluator.gameUI.TimeCounter;
            timeTmp.text = "0";
            
            results = new EvaluationResults();

            beginTest();
            
            targetsProvider.PickNextTarget();
            while (testing)
            {
                var delta = Time.deltaTime;
                results.currentTime += delta;
                allTime += delta;
                timeTmp.text = $"{allTime:0.##}";
                yield return null;
            }

            endTest();
        }

        protected override void onClick(SelectableTarget target)
        {
            base.onClick(target);
            evaluator.gameUI.TargetsCounter.text = results.selectedCount.ToString();
        }
    }
}