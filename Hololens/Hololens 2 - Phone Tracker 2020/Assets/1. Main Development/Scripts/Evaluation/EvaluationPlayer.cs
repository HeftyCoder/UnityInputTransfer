using System;
using System.Collections.Generic;

namespace UOPHololens.Evaluation
{
    [Serializable]
    public class EvaluationPlayer
    {
        [NonSerialized] public string username;
        
        public int age;
        public List<EvaluationResults> targetBasedEvaluation = new List<EvaluationResults>();
        public List<EvaluationResults> timeBasedEvaluation = new List<EvaluationResults>();
    }

    [Serializable]
    public class EvaluationResults
    {
        private float currentLookupTime;
        private List<float> currentLookUpTimesList;
        private List<float> currentSelectedTimesList;
        public int targetsHit = 0;
        public List<List<float>> targetLookUpTimes = new List<List<float>>();
        public List<float> targetSelectedTimes = new List<float>();

        public void LookedAtTarget()
        {

        }
        public void TargetSelected()
        {

        }
    }
}