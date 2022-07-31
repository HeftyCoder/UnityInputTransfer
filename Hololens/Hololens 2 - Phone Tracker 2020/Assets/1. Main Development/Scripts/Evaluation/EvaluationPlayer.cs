using System;
using System.Collections;
using System.Collections.Generic;
namespace UOPHololens.Evaluation
{
    [Serializable]
    public class EvaluationPlayer
    {
        [NonSerialized] public string username;
        
        public int age;
        public EvaluationTest targetBasedTest = new EvaluationTest();
        public EvaluationTest timeBasedTest = new EvaluationTest();
    
    }

    [Serializable]
    public class EvaluationTest
    {
        public List<EvaluationResults> nativeTest = new List<EvaluationResults>();
        public List<EvaluationResults> phoneTest = new List<EvaluationResults>();
    }
    [Serializable]
    public class EvaluationResults
    {
        [NonSerialized] public float currentTime = 0;
        private LookUpTimes currentLookUpTimesList;

        public int selectedCount = 0;
        public List<LookUpTimes> targetLookUpTimes = new List<LookUpTimes>();
        public List<float> targetSelectedTimes = new List<float>();

        public EvaluationResults()
        {
            currentLookUpTimesList = new LookUpTimes();
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
            currentLookUpTimesList = new LookUpTimes();
            targetLookUpTimes.Add(currentLookUpTimesList);
            targetSelectedTimes.Add(currentTime);
            currentTime = 0;
        }
    }

    //This is needed for Unity's serialization. It can't serialize List<List<..>> on its own
    [Serializable]
    public class LookUpTimes: IList<float>
    {
        public List<float> times = new List<float>();
        public float this[int index] { get => times[index]; set => times[index] = value; }
        public int Count => times.Count;
        public bool IsReadOnly => false;
        public void Add(float item) => times.Add(item);
        public void Clear() => times.Clear();
        public bool Contains(float item) => times.Contains(item);
        public void CopyTo(float[] array, int arrayIndex) => times.CopyTo(array, arrayIndex);
        public IEnumerator<float> GetEnumerator() => times.GetEnumerator();
        public int IndexOf(float item) => times.IndexOf(item);
        public void Insert(int index, float item) => times.Insert(index, item);
        public bool Remove(float item) => times.Remove(item);
        public void RemoveAt(int index) => times.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)times).GetEnumerator();
        }
    }
}