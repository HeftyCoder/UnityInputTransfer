using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

namespace UOPHololens.Evaluation
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text targetsLeftCounter;
        [SerializeField] private TMP_Text basicTestInformation;
        [SerializeField] private TMP_Text timeCounter;
        [SerializeField] PressableButtonHoloLens2 button;
        public TMP_Text TargetsCounter => targetsLeftCounter;
        public TMP_Text TestInformation => basicTestInformation;
        public TMP_Text TimeCounter => timeCounter;
        public PressableButtonHoloLens2 ContinueButton => button;

    }
}


