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
        [SerializeField] ButtonConfigHelper exitButton;

        private void Awake()
        {
            exitButton.OnClick.AddListener(() => StopTest());
        }
        public override IEnumerator StartTest()
        {
            exitButton.gameObject.SetActive(true);
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
            exitButton.gameObject.SetActive(false);
        }

        public override void StopTest() => testing = false;

        protected override void onClick(SelectableTarget target)
        {
            base.onClick(target);
            evaluator.gameUI.TargetsCounter.text = results.selectedCount.ToString();
        }
    }
}