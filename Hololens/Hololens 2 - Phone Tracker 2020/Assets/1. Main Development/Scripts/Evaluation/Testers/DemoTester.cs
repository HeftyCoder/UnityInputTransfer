using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace UOPHololens.Evaluation
{
    public class DemoTester : BaseTester
    {
        [SerializeField] bool testing = false;
        public override IEnumerator StartTest()
        {
            var targetsProvider = evaluator.targetsProvider;
            var player = evaluator.player;
            var demoResults = new EvaluationResults();
            evaluator.currentEvaluation = demoResults;

            targetsProvider.SetActiveStateTargets(true);
            targetsProvider.EnableTargets(false);

            void onClick(SelectableTarget target)
            {
                demoResults.TargetSelected();
                targetsProvider.PickNextTarget();
            }
            void onFocusEnter(SelectableTarget target) => demoResults.LookedAtTarget();
            void onFocusExit(SelectableTarget target) => demoResults.LookedAwayFromTarget();

            targetsProvider.AddOnClick(onClick);
            targetsProvider.AddOnFocusExit(onFocusExit);
            targetsProvider.AddOnFocusEnter(onFocusEnter);

            testing = true;
            yield return beginTest();

            targetsProvider.PickNextTarget();
            while (testing)
            {
                demoResults.currentTime += Time.deltaTime;
                yield return null;
            }

            targetsProvider.RemoveOnClick(onClick);
            targetsProvider.RemoveOnFocusEnter(onFocusEnter);
            targetsProvider.RemoveOnFocusExit(onFocusExit);

            targetsProvider.EnableTargets(false);
            targetsProvider.SetActiveStateTargets(false);

            evaluator.Save();
        }
    }
}