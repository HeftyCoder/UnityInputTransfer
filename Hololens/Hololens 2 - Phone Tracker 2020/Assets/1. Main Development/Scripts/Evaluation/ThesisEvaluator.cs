using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using Microsoft.MixedReality.Toolkit.Input;

namespace UOPHololens.Evaluation
{
    public class ThesisEvaluator : MonoBehaviour
    {
        public enum State { Idle, Loading, Saving, Evaluating }

        [SerializeField] internal SimpleFirebaseClient client;
        [SerializeField] internal GameUI gameUI;
        [SerializeField] internal UserUI userUI;
        [SerializeField] internal GameObject mainMenu;
        [SerializeField] GameObject introUI;
        [SerializeField] ThesisInputHandler inputHandler;
        [SerializeField] GazeProvider gazeProvider;
        [SerializeField] bool enableEyeTracking = false;
        [SerializeField] internal string path = "testers";

        [Header("Targets provider")]
        [SerializeField] internal SelectableTargetsProvider targetsProvider;
        [SerializeField] AnchorKeeper anchorKeeper;
        [SerializeField] SelectableTarget targetPrefab;
        [SerializeField] List<SelectableTarget> targetsIfNoAnchors = new List<SelectableTarget>();

        private List<SelectableTarget> createdTargets = new List<SelectableTarget>();
        internal EvaluationPlayer player = new EvaluationPlayer();
        private string username = "george";
        private BaseTester currentTester;
        State state = State.Idle;
        Coroutine currentTest;

        public EvaluationResults currentEvaluation;

        public ThesisInputHandler InputHandler => inputHandler;
        private void Awake()
        {
            gazeProvider.IsEyeTrackingEnabled = enableEyeTracking;
            currentTester = null;
            userUI.UsernameInput.onSubmit.AddListener((username) =>
            {
                state = State.Loading;
                this.username = username;
                client.Get<EvaluationPlayer>(getPath(username), (player, valid) =>
                {
                    Debug.Log(this.username);
                    Debug.Log(valid);
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
            currentTester?.Menu.SetActive(false);
            foreach (var target in targetsIfNoAnchors)
            {
                target.gameObject.SetActive(false);
            }
        }
        public State CurrentState => state;

        public void Play()
        {
            if (Protect())
                return;
            state = State.Evaluating;
            IEnumerator play()
            {
                CreateTargetsFromAnchors();
                yield return null;
                if (createdTargets.Count != 0)
                    targetsProvider.targets = createdTargets;
                else
                    targetsProvider.targets = targetsIfNoAnchors;
                SetTest();
                userUI.gameObject.SetActive(false);

                currentTester.evaluator = this;
                yield return currentTester.StartTest();
                state = State.Idle;

                gameUI.gameObject.SetActive(false);
                currentTest = null;
                SetMain();
                ClearTargetsFromAnchors();
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
            ClearTargetsFromAnchors();
        }
        private void CreateTargetsFromAnchors()
        {
            foreach (var anchor in anchorKeeper.Anchors)
            {
                var target = Instantiate(targetPrefab);
                var tr = anchor.transform;
                target.transform.SetPositionAndRotation(tr.position, tr.rotation);
                createdTargets.Add(target);
            }
        }
        private void ClearTargetsFromAnchors()
        {
            foreach (var target in createdTargets)
                Destroy(target.gameObject);
            createdTargets.Clear();
        }
        public void Save(Action<string, bool> onResult = null)
        {
            if (player == null)
                return;
            if (int.TryParse(userUI.AgeInput.text, out int age))
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

