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
        public override IEnumerator StartTest()
        {
            var timeCounter = evaluator.gameUI.TimeCounter;
            float fullTime = 0;

            IEnumerator doTest()
            {
                currentCount = targetCount;
                evaluator.gameUI.Open(targetCount, 0);
                
                targetsProvider.PickNextTarget();
                while (currentCount > 0)
                {
                    var delta = Time.deltaTime;
                    fullTime += delta;
                    results.currentTime += delta;
                    timeCounter.text = fullTime.ToString();
                    yield return null;
                }

                endingSound.Play();
                yield return new WaitForSeconds(endingSound.clip.length);
                evaluator.gameUI.Close();
            }

            beginTest();

            yield return StartPhase?.Wait();

            var test = evaluator.player.targetBasedTest;
            var firstPhase = GetFirstPhase(test);
            yield return firstPhase.Wait();
            yield return doTest();
            
            yield return MiddlePhase?.Wait();

            var secondPhase = GetSecondPhase(test);
            yield return secondPhase.Wait();
            yield return doTest();

            yield return EndPhase?.Wait();
            endTest();
        }
        protected override void onClick(SelectableTarget target)
        {
            base.onClick(target);
            currentCount--;
            evaluator.gameUI.TargetsCounter.text = currentCount.ToString();
        }
    }
}