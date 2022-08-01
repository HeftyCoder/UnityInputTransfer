using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace UOPHololens.Evaluation
{
    public class SpecificTargetTester : PhaseTester
    {
        public int targetCount = 15;
        private int currentCount;
        private bool earlyExit = false;
        public override IEnumerator StartTest()
        {
            earlyExit = false;
            var timeCounter = evaluator.gameUI.TimeCounter;
            float fullTime = 0;

            IEnumerator doTest()
            {
                fullTime = 0;
                currentCount = targetCount;
                evaluator.gameUI.Open(targetCount, 0);
                
                targetsProvider.PickNextTarget();
                while (currentCount > 0)
                {
                    var delta = Time.deltaTime;
                    fullTime += delta;
                    results.currentTime += delta;
                    timeCounter.text = $"{fullTime:0.##}";
                    yield return null;
                }


                endingSound.Play();
                targetsProvider.EnableTargets(false);
                evaluator.gameUI.Close();
            }

            beginTest();
            targetsProvider.EnableTargets(false);

            yield return StartPhase?.Wait();

            var test = evaluator.player.targetBasedTest;
            var firstPhase = GetFirstPhase(test);
            yield return firstPhase.Wait();
            yield return doTest();
            results.fullTime = fullTime;

            yield return MiddlePhase?.Wait();

            var secondPhase = GetSecondPhase(test);
            yield return secondPhase.Wait();
            yield return doTest();

            results.fullTime = fullTime;
            evaluator.Save();
            yield return EndPhase?.Wait();
            endTest();
            endingSound.Stop();
        }
        protected override void onClick(SelectableTarget target)
        {
            currentCount--;
            if (currentCount < 0)
                return;
            evaluator.gameUI.TargetsCounter.text = currentCount.ToString();
            base.onClick(target);
        }
    }
}