using System.Collections;
using UnityEngine;

namespace UOPHololens.Evaluation
{
    public class CountdownTester : BaseTester
    {
        public float allowedTime = 20;
        private float currentTime;
        public override IEnumerator StartTest()
        {
            currentTime = allowedTime;
            var timeTmp = evaluator.gameUI.TimeCounter;
            timeTmp.text = allowedTime.ToString();
            yield return beginTest();
            
            targetsProvider.PickNextTarget();

            while (currentTime > 0)
            {
                var delta = Time.deltaTime;
                results.currentTime += delta;
                currentTime -= delta;
                timeTmp.text = currentTime.ToString();
                yield return null;
            }

            yield return endTest();
        }
        protected override void onClick(SelectableTarget target)
        {
            base.onClick(target);
            evaluator.gameUI.TargetsCounter.text = results.selectedCount.ToString();
        }
    }
}