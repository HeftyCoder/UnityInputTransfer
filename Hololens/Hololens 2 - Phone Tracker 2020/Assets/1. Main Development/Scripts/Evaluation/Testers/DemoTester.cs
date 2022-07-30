using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace UOPHololens.Evaluation
{
    public abstract class DemoTester : BaseTester
    {
        bool testing = false;
        public override IEnumerator StartTest()
        {
            var targetsProvider = evaluator.targetsProvider;
            foreach (var target in targetsProvider.targets)
            {
                target.onClicked 
            }
            testing = true;
            yield return beginTest();
            while (testing)
            {

            }
        }
    }
}