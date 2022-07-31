using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace UOPHololens.Evaluation
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text targetsLeftCounter;
        [SerializeField] private TMP_Text basicTestInformation;
        [SerializeField] private TMP_Text timeCounter;
        public TMP_Text TargetsCounter => targetsLeftCounter;
        public TMP_Text TestInformation => basicTestInformation;
        public TMP_Text TimeCounter => timeCounter;
    }
}


