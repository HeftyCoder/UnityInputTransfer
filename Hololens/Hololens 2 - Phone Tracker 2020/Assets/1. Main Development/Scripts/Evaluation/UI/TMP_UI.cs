using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UOPHololens.Evaluation
{
    public class TMP_UI : MonoBehaviour
    {
        [SerializeField] TMP_Text tmp;
        public TMP_Text TextMeshPro => tmp;
    }
}
