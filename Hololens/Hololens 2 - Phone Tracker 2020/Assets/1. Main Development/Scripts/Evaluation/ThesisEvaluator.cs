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

        [SerializeField] internal SimpleFirebaseClient client;
        [SerializeField] internal GameUI gameUI;
        [SerializeField] internal UserUI userUI;
        [SerializeField] internal GameObject mainMenu;
        [SerializeField] internal SelectableTargetsProvider targetsProvider;
        [SerializeField] internal string path = "testers";

        public EvaluationResults currentEvaluation;

        internal EvaluationPlayer player = new EvaluationPlayer();
        private string username = "george";
        private BaseTester currentTester;
        State state = State.Idle;

        Coroutine currentTest;
        private void Awake()
        {
            currentTester = null;
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
        private void Start()
        {
            SetMain();
        }
        public State CurrentState => state;

        public void Play()
        {
            if (Protect())
                return;
            state = State.Evaluating;
            IEnumerator play()
            {
                yield return null;
                SetTest();
                userUI.gameObject.SetActive(false);

                currentTester.evaluator = this;
                yield return currentTester.StartTest();
                state = State.Idle;

                gameUI.gameObject.SetActive(false);
                currentTest = null;
                SetMain();
            }

            currentTest = StartCoroutine(play());
        }
        public void Stop()
        {
            if (state == State.Idle || currentTest == null)
                return;

            state = State.Idle;
            StopCoroutine(currentTest);
            SetMain();
            targetsProvider.EnableTargets(false);
            targetsProvider.SetActiveStateTargets(false);
            gameUI.gameObject.SetActive(false);
            currentTester?.Stop();
            currentTester = null;
            currentTest = null;
        }
        public void Save(Action<string, bool> onResult = null)
        {
            if (player == null)
                return;
            if (int.TryParse(userUI.AgeInput.text, out int age)) ;
            player.age = age;

            client.Save(getPath(username), player, (data, valid) =>
            {
                onResult?.Invoke(data, valid);
            });
        }

        public void Play(BaseTester tester)
        {
            SetTester(tester);
            Play();
        }
        public void SetTester(BaseTester tester) => currentTester = tester;
        private void SetMain()
        {
            mainMenu.SetActive(true);
            currentTester?.Menu.SetActive(false);
        }
        private void SetTest()
        {
            mainMenu.SetActive(false);
            currentTester?.Menu.SetActive(true);
        }
        private bool Protect() => state != State.Idle || !enabled;
        private string getPath(string username) => $"{path}/{username}";
    }
}

