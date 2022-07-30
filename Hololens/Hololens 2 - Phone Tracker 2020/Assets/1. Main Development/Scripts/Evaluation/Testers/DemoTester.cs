using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace UOPHololens.Evaluation
{
    public abstract class DemoTester : BaseTester
    {
        bool testing = false;
        EvaluationResults demoResults;
        public override IEnumerator StartTest()
        {
            var targetsProvider = evaluator.targetsProvider;
            var player = evaluator.player;
            demoResults = new EvaluationResults();

            targetsProvider.EnableTargets(true);

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

            while (testing)
            {
                demoResults.currentTime += Time.deltaTime;
                yield return null;
            }

            targetsProvider.RemoveOnClick(onClick);
            targetsProvider.RemoveOnFocusEnter(onFocusEnter);
            targetsProvider.RemoveOnFocusExit(onFocusExit);

            targetsProvider.EnableTargets(false);
        }
    }
}