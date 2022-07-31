using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

namespace UOPHololens.Evaluation
{
    public class TestPhaseHelper : MonoBehaviour    
    {
        static WaitForSeconds waitOne = new WaitForSeconds(1);
        
        public int countdown;
        public bool showButton;
        public bool showCountdown;

        [SerializeField] TMP_Text[] infoTmps;
        [SerializeField] TMP_Text countdownInfoTmp;
        [SerializeField] TMP_Text countdownTmp;
        [SerializeField] ButtonConfigHelper configHelper;

        bool continueTest = false;
        private void Start()
        {
            countdownInfoTmp.enabled = false;
            countdownTmp.enabled = false;
            configHelper.OnClick.AddListener(() => continueTest = true);
        }
        
        
        public IEnumerator Wait(int countdown = -1)
        {
            gameObject.SetActive(true);

            foreach (var tmp in infoTmps)
                tmp.enabled = true;

            configHelper.gameObject.SetActive(showButton);
            continueTest = false;

            while (!continueTest && showButton)
                yield return null;

            configHelper.gameObject.SetActive(false);
            if (countdown > 0)
            {
                foreach (var tmp in infoTmps)
                    tmp.enabled = false;

                countdownInfoTmp.enabled = showCountdown;
                countdownTmp.enabled = showCountdown;

                var count = countdown;
                countdownTmp.text = count.ToString();
                countdownTmp.enabled = true;

                while (count >= 0)
                {
                    count -= 1;
                    countdownTmp.text = count.ToString();
                    yield return waitOne;
                }

                countdownTmp.enabled = false;
                countdownInfoTmp.enabled = false;
            }

            gameObject.SetActive(false);
        }

    }
}