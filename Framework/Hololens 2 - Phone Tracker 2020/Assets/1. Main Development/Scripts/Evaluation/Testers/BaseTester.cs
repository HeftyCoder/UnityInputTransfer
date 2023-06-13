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
        [SerializeField] GameObject[] testerMenuObjects;

        protected WaitForSeconds waitOne = new WaitForSeconds(1);
        protected SelectableTargetsProvider targetsProvider;
        protected EvaluationResults results;

        private void Start()
        {
            SetMenu(false);
        }

        public virtual IEnumerator StartTest()
        {
            yield return null;
        }
        public virtual void Stop()
        {
            endTest();
        }
        protected void beginTest()
        {
            targetsProvider = evaluator.targetsProvider;
            targetsProvider.SetActiveStateTargets(true);
            targetsProvider.EnableTargets(false);
            targetsProvider.AddOnClick(onClick);
            targetsProvider.AddOnFocusExit(onFocusExit);
            targetsProvider.AddOnFocusEnter(onFocusEnter);
            SetMenu(true);
        }
        protected void endTest()
        {
            targetsProvider.RemoveOnClick(onClick);
            targetsProvider.RemoveOnFocusEnter(onFocusEnter);
            targetsProvider.RemoveOnFocusExit(onFocusExit);
            targetsProvider.EnableTargets(false);
            targetsProvider.SetActiveStateTargets(false);
            SetMenu(false);
        }

        protected virtual void onClick(SelectableTarget target)
        {
            results.TargetSelected();
            targetsProvider.PickNextTarget();
        }
        void onFocusEnter(SelectableTarget target) => results.LookedAtTarget();
        void onFocusExit(SelectableTarget target) => results.LookedAwayFromTarget();
    
        private void SetMenu(bool enable)
        {
            foreach (var go in testerMenuObjects)
                go.SetActive(enable);
        }
    }
}
