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
                    timeTmp.text = currentTime.ToString();
                    yield return null;
                }
                endingSound.Play();
                yield return new WaitForSeconds(endingSound.clip.length);
                evaluator.gameUI.Close();
            }

            beginTest();

            yield return StartPhase?.Wait();
            //First phase
            var test = evaluator.player.timeBasedTest;
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
            evaluator.gameUI.TargetsCounter.text = results.selectedCount.ToString();
        }
    }
}