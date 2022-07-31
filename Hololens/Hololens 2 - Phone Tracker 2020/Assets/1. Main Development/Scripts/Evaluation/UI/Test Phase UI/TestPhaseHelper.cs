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

        [SerializeField] TMP_Text infoTmp;
        [SerializeField] TMP_Text countdownTmp;
        [SerializeField] ButtonConfigHelper configHelper;

        bool continueTest = false;
        private void Start()
        {
            gameObject.SetActive(false);
            countdownTmp.enabled = false;
            configHelper.OnClick.AddListener(() => continueTest = true);
        }
        
        
        public IEnumerator WaitPhase(int countdown = -1)
        {
            gameObject.SetActive(true);
            var onClick = configHelper.OnClick;
            var waiter = countdown <= 0 ? new WaitForSeconds(this.countdown) : new WaitForSeconds(countdown);
            configHelper.gameObject.SetActive(showButton);

            while (!continueTest && showButton)
                yield return null;

            if (countdown > 0)
            {
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
            }

            gameObject.SetActive(false);
        }

    }
}