using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace UOPHololens.Evaluation
{
    public class SelectableTargetsProvider : MonoBehaviour
    {
        public List<SelectableTarget> targets = new List<SelectableTarget>();

        public SelectableTarget PickRandomTarget()
        {
           var randomIndex =  UnityEngine.Random.RandomRange(0, targets.Count);
            return targets[randomIndex];
        }
        public void AddOnClick(Action<GameObject> onClick)
        {
            foreach (var target in targets)
                target.onClicked += onClick;
        }
        public void RemoveOnClick(Action<GameObject> onClick)
        {
            foreach (var target in targets)
                target.onClicked -= onClick;
        }
        public void EnableTargets(bool enable)
        {
            foreach (var target in targets)
                target.enabled = enable;
        }
    }
}
