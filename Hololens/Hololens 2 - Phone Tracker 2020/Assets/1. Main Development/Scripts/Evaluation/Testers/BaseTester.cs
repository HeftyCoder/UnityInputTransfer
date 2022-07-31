﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace UOPHololens.Evaluation
{
    public abstract class BaseTester : MonoBehaviour
    {
        [NonSerialized] public ThesisEvaluator evaluator;
        public string startText;
        public int countBeforeStart;
        public string endText;
        public int countBeforeEnd;

        protected WaitForSeconds waitOne = new WaitForSeconds(1);
        protected SelectableTargetsProvider targetsProvider;
        protected EvaluationResults results;
        public virtual IEnumerator StartTest()
        {
            yield return null;
        }
        public virtual void StopTest()
        {

        }
        protected IEnumerator beginTest()
        {
            targetsProvider = evaluator.targetsProvider;
            targetsProvider.SetActiveStateTargets(true);
            targetsProvider.EnableTargets(false);
            targetsProvider.AddOnClick(onClick);
            targetsProvider.AddOnFocusExit(onFocusExit);
            targetsProvider.AddOnFocusEnter(onFocusEnter);
            yield return showInfo(startText, countBeforeStart);
        }
        protected IEnumerator endTest()
        {
            targetsProvider.RemoveOnClick(onClick);
            targetsProvider.RemoveOnFocusEnter(onFocusEnter);
            targetsProvider.RemoveOnFocusExit(onFocusExit);
            targetsProvider.EnableTargets(false);
            targetsProvider.SetActiveStateTargets(false);
            yield return showInfo(endText, countBeforeEnd);
        }
        protected IEnumerator showInfo(string info, int countdown)
        {
            var infoTmp = evaluator.gameUI.TestInformation;
            infoTmp.enabled = true;
            infoTmp.text = info;
            for (int i = 0; i < countdown; i++)
                yield return waitOne;

            for (int i = countdown; i > 0; i--)
            {
                infoTmp.text = $"{i}";
                yield return waitOne;
            }
                
            infoTmp.enabled = false;
        }

        protected virtual void onClick(SelectableTarget target)
        {
            results.TargetSelected();
            targetsProvider.PickNextTarget();
        }
        void onFocusEnter(SelectableTarget target) => results.LookedAtTarget();
        void onFocusExit(SelectableTarget target) => results.LookedAwayFromTarget();
    }
}