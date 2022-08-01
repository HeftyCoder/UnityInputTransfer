using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;

namespace UOPHololens.Evaluation
{
    public class ThesisEvaluator : MonoBehaviour
    {
        public enum State { Idle, Loading, Saving, Evaluating }

        public BaseTester evaluationTest;

        [SerializeField] internal SimpleFirebaseClient client;
        [SerializeField] internal GameUI gameUI;
        [SerializeField] internal UserUI userUI;
        [SerializeField] internal SelectableTargetsProvider targetsProvider;
        [SerializeField] internal string path = "testers";

        public EvaluationResults currentEvaluation;

        internal EvaluationPlayer player = new EvaluationPlayer();
        private string username = "george";
        State state = State.Idle;

        private void Awake()
        {
            userUI.UsernameInput.onSubmit.AddListener((username) =>
            {
                state = State.Loading;
                this.username = username;
                client.Get<EvaluationPlayer>(getPath(username), (player, valid) =>
                {
                    this.username = username;
                    if (!valid)
                        this.player = new EvaluationPlayer() { username = username };
                    else
                    {
                        this.player = player;
                        userUI.AgeInput.text = player.age.ToString();
                    }
                    state = State.Idle;
                });
            });     
        }
        public State CurrentState => state;

        [ContextMenu("Play")]
        public void Play()
        {
            if (Protect())
                return;
            state = State.Evaluating;
            IEnumerator play()
            {
                userUI.gameObject.SetActive(false);

                evaluationTest.evaluator = this;
                yield return evaluationTest.StartTest();
                state = State.Idle;

                gameUI.gameObject.SetActive(false);
            }

            StartCoroutine(play());
        }

        public void Save(Action<string, bool> onResult = null)
        {
            if (player == null)
                return;
            if (int.TryParse(userUI.AgeInput.text, out int age));
                player.age = age;

            client.Save(getPath(username), player, (data, valid) =>
            {
                onResult?.Invoke(data, valid);
            });
        }
        private bool Protect() => state != State.Idle || !enabled || evaluationTest == null;
        private string getPath(string username) => $"{path}/{username}";
    }
}

