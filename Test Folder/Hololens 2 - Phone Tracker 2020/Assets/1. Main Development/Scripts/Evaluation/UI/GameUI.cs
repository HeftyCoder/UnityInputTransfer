using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

namespace UOPHololens.Evaluation
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] GameObject targetsLeftContainer;
        [SerializeField] private TMP_Text targetsLeftCounter;
        [SerializeField] GameObject timeContainer;
        [SerializeField] private TMP_Text timeCounter;
        public TMP_Text TargetsCounter => targetsLeftCounter;
        public TMP_Text TimeCounter => timeCounter;

        public void Open(int targetsCount, float time)
        {
            gameObject.SetActive(true);
            targetsLeftContainer.SetActive(true);
            timeContainer.SetActive(true);
            targetsLeftCounter.text = targetsCount.ToString();
            timeCounter.text = time.ToString();
        }
        public void Open(int targetsCount)
        {
            gameObject.SetActive(true);
            targetsLeftContainer.SetActive(true);
            timeContainer.SetActive(false);
            targetsLeftCounter.text = targetsCount.ToString();
        }
        public void Open(float time)
        {
            gameObject.SetActive(true);
            targetsLeftContainer.SetActive(false);
            timeContainer.SetActive(true);
            timeCounter.text = time.ToString();
        }
        public void Close() => gameObject.SetActive(false);

    }
}


