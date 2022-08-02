using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace UOPHololens.Evaluation
{
    public class UserUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField usernameInput, ageInput;

        [SerializeField] AnchorKeeper anchorKeeper;
        [SerializeField] GameObject mainSettings;
        [SerializeField] GameObject anchorSettings;
        public TMP_InputField UsernameInput => usernameInput;
        public TMP_InputField AgeInput => ageInput;

        public void EnterAnchorKeeper()
        {
            mainSettings.SetActive(false);
            anchorSettings.SetActive(true);
            anchorKeeper.OpenAnchorEdit();
        }
        public void ExitAnchorKeeper()
        {
            mainSettings.SetActive(true);
            anchorSettings.SetActive(false);
            anchorKeeper.ExitAnchorEdit();
        }
        public void SaveAndExit()
        {
            anchorKeeper.Save();
            ExitAnchorKeeper();
        }
    }
}

