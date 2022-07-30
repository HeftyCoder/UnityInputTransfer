using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

namespace UOPHololens.Evaluation
{
    public class SelectableTargetsProvider : MonoBehaviour
    {
        [SerializeField] DirectionalIndicator targetIndicator;
        public List<SelectableTarget> targets = new List<SelectableTarget>();

        public void PickNextTarget()
        {
            var randomTarget = PickRandomTarget();
            foreach (var target in targets)
            {
                if (target == randomTarget)
                    target.enabled = true;
                else
                    target.enabled = false;
            }
            targetIndicator.DirectionalTarget = randomTarget.transform;
        }
        public SelectableTarget PickRandomTarget()
        {
            var randomIndex =  UnityEngine.Random.Range(0, targets.Count);
            return targets[randomIndex];
        }
        public void AddOnClick(Action<SelectableTarget> onClick)
        {
            foreach (var target in targets)
                target.onClicked += onClick;
        }
        public void RemoveOnClick(Action<SelectableTarget> onClick)
        {
            foreach (var target in targets)
                target.onClicked -= onClick;
        }
        public void AddOnFocusEnter(Action<SelectableTarget> onFocus)
        {
            foreach (var target in targets)
                target.onFocusEnter += onFocus;
        }
        public void RemoveOnFocusEnter(Action<SelectableTarget> onFocus)
        {
            foreach (var target in targets)
                target.onFocusEnter -= onFocus;
        }
        public void AddOnFocusExit(Action<SelectableTarget> onFocus)
        {
            foreach (var target in targets)
                target.onFocusExit += onFocus;
        }
        public void RemoveOnFocusExit(Action<SelectableTarget> onFocus)
        {
            foreach (var target in targets)
                target.onFocusExit -= onFocus;
        }
        public void EnableTargets(bool enable)
        {
            foreach (var target in targets)
                target.enabled = enable;
        }
    }
}
