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
        public float currentTime = 0;
        private List<float> currentLookUpTimesList;
        public int selectedCount = 0;
        public List<List<float>> targetLookUpTimes = new List<List<float>>();
        public List<float> targetSelectedTimes = new List<float>();

        public EvaluationResults()
        {
            currentLookUpTimesList = new List<float>();
            targetLookUpTimes.Add(currentLookUpTimesList);
        }

        public void LookedAtTarget()
        {
            currentLookUpTimesList.Add(currentTime);
            currentTime = 0;
        }
        public void LookedAwayFromTarget()
        {
            currentTime = 0;
        }
        public void TargetSelected()
        {
            selectedCount++;
            currentLookUpTimesList = new List<float>();
            targetLookUpTimes.Add(currentLookUpTimesList);
            targetSelectedTimes.Add(currentTime);
            currentTime = 0;
        }
    }
}