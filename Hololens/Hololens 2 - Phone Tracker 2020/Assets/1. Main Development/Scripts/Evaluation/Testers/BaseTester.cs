using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Microsoft.MixedReality.Toolkit.UI;

namespace UOPHololens.Evaluation
{
    public abstract class BaseTester : MonoBehaviour
    {
        [NonSerialized]  public ThesisEvaluator evaluator;
        [SerializeField] protected AudioSource endingSound;
        [SerializeField] GameObject testerMenu;

        protected WaitForSeconds waitOne = new WaitForSeconds(1);
        protected SelectableTargetsProvider targetsProvider;
        protected EvaluationResults results;

        public GameObject Menu => testerMenu;
        public virtual IEnumerator StartTest()
        {
            yield return null;
        }
        protected void beginTest()
        {
            targetsProvider = evaluator.targetsProvider;
            targetsProvider.SetActiveStateTargets(true);
            targetsProvider.EnableTargets(false);
            targetsProvider.AddOnClick(onClick);
            targetsProvider.AddOnFocusExit(onFocusExit);
            targetsProvider.AddOnFocusEnter(onFocusEnter);
        }
        protected void endTest()
        {
            targetsProvider.RemoveOnClick(onClick);
            targetsProvider.RemoveOnFocusEnter(onFocusEnter);
            targetsProvider.RemoveOnFocusExit(onFocusExit);
            targetsProvider.EnableTargets(false);
            targetsProvider.SetActiveStateTargets(false);
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
