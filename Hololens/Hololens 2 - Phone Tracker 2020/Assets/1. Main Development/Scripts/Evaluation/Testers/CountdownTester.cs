using System.Collections;
using UnityEngine;

namespace UOPHololens.Evaluation
{
    public class CountdownTester : PhaseTester
    {
        public float allowedTime = 20;

        private float currentTime;
        public override IEnumerator StartTest()
        {
            var timeTmp = evaluator.gameUI.TimeCounter;
            IEnumerator doTest()
            {
                currentTime = allowedTime;
                evaluator.gameUI.Open(0, allowedTime);

                targetsProvider.PickNextTarget();
                while (currentTime > 0)
                {
                    var delta = Time.deltaTime;
                    results.currentTime += delta;
                    currentTime -= delta;
                    timeTmp.text = $"{currentTime:0.##}";
                    yield return null;
                }
                endingSound.Play();
                evaluator.gameUI.Close();
                targetsProvider.EnableTargets(false);
            }

            beginTest();
            targetsProvider.EnableTargets(false);

            yield return StartPhase?.Wait();
            //First phase
            var test = evaluator.player.timeBasedTest;
            var firstPhase = GetFirstPhase(test);
            yield return firstPhase.Wait();
            yield return doTest();
            results.fullTime = allowedTime;

            yield return MiddlePhase?.Wait();

            var secondPhase = GetSecondPhase(test);
            yield return secondPhase.Wait();
            yield return doTest();

            results.fullTime = allowedTime;
            evaluator.Save();
            yield return EndPhase?.Wait();

            endingSound.Stop();
            endTest();
        }

        protected override void onClick(SelectableTarget target)
        {
            base.onClick(target);
            evaluator.gameUI.TargetsCounter.text = results.selectedCount.ToString();
        }
    }
}