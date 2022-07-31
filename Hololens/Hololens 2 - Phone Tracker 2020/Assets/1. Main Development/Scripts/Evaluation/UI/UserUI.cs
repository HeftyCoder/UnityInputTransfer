using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace UOPHololens.Evaluation
{
    public class UserUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField usernameInput, ageInput;
        public TMP_InputField UsernameInput => usernameInput;
        public TMP_InputField AgeInput => ageInput;
    }
}

