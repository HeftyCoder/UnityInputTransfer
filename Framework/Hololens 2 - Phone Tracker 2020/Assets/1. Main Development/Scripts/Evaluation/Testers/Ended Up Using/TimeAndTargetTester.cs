using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UOPHololens.Evaluation
{
    public class TimeAndTargetTester : BaseTester
    {
        public float timePhaseTime = 45;
        public int targetPhaseTargets = 30;

        [SerializeField] bool isNativeTest = true;
        [SerializeField] TestPhaseHelper startPhase;
        [SerializeField] TestPhaseHelper timePhase;
        [SerializeField] TestPhaseHelper middlePhase;
        [SerializeField] TestPhaseHelper targetPhase;
        [SerializeField] TestPhaseHelper endPhase;

        bool isTimeTesting = false; 
        private float currentTime = 0;
        private int currentCount;
        private TMP_Text timeTmp => evaluator.gameUI.TimeCounter;
        private TMP_Text targetTmp => evaluator.gameUI.TargetsCounter;
        public override IEnumerator StartTest()
        {
            beginTest();
            yield return startPhase?.Wait();
            isTimeTesting = true;
            results = new EvaluationResults();
            var timeTest = evaluator.player.timeBasedTest;
            if (isNativeTest)
                timeTest.nativeTest.Add(results);
            else
                timeTest.phoneTest.Add(results);

            yield return timePhase.Wait();
            yield return doTimeTest();
            results.fullTime = timePhaseTime;
            yield return middlePhase?.Wait();

            isTimeTesting = false;
            var targetTest = evaluator.player.targetBasedTest;
            results = new EvaluationResults();
            if (isNativeTest)
                targetTest.nativeTest.Add(results);
            else
                targetTest.phoneTest.Add(results);

            yield return targetPhase.Wait();
            yield return doTargetTest();
            results.fullTime = currentTime;

            evaluator.Save();
            endingSound.Play();
            endTest();
            yield return endPhase?.Wait();
        }
        private IEnumerator doTimeTest()
        {
            currentTime = timePhaseTime;
            currentCount = 0;
            evaluator.gameUI.Open(0, timePhaseTime);

            targetsProvider.PickNextTarget();
            while (currentTime > 0)
            {
                var delta = Time.deltaTime;
                results.currentTime += delta;
                currentTime -= delta;
                timeTmp.text = $"{currentTime:0.00}";
                yield return null;
            }

            targetsProvider.EnableTargets(false);
        }
        private IEnumerator doTargetTest()
        {
            currentTime = 0;
            currentCount = targetPhaseTargets;
            evaluator.gameUI.Open(currentCount, 0);

            targetsProvider.PickNextTarget();
            while (currentCount > 0)
            {
                var delta = Time.deltaTime;
                currentTime += delta;
                results.currentTime += delta;
                timeTmp.text = $"{currentTime:0.00}";
                yield return null;
            }

            targetsProvider.EnableTargets(false);
        }
        protected override void onClick(SelectableTarget target)
        {
            if (!isTimeTesting)
            {
                currentCount--;
                if (currentCount < 0)
                    return;
                targetTmp.text = currentCount.ToString();
                base.onClick(target);
            }
            else
            {
                base.onClick(target);
                targetTmp.text = results.selectedCount.ToString();
            }
            
        }

        public override void Stop()
        {
            base.Stop();
            isTimeTesting = false;
            startPhase?.Close();
            timePhase?.Close();
            middlePhase?.Close();
            targetPhase?.Close();
            endPhase?.Close();
        }

    }
}

