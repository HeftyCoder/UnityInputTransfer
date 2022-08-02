using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.OpenXR.ARFoundation;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using System;

namespace UOPHololens.Evaluation
{

    public class AnchorKeeper : MonoBehaviour
    {
        private const string anchorKeeperID = "ANCHORS";

        [SerializeField] Transform prefabForVisualization;
        [SerializeField] ARAnchorManager anchorManager;
        [SerializeField] TMP_Text status;
        [SerializeField] Camera mainCamera;

        private XRAnchorStore anchorStore;
        private SavedAnchors savedAnchorNames = new SavedAnchors();
        private HashSet<Transform> trackables = new HashSet<Transform>();
        private HashSet<ARAnchor> anchors = new HashSet<ARAnchor>();
        public IEnumerable<ARAnchor> Anchors => anchors;
        private async void Start()
        {
            //https://docs.microsoft.com/en-us/dotnet/api/microsoft.mixedreality.openxr.arfoundation.anchormanagerextensions.loadanchorstoreasync?view=mixedreality-openxr-plugin-1.4
            //Why was this so hard to find
            try
            {
                anchorStore = await anchorManager.LoadAnchorStoreAsync();
                status.text = (anchorStore == null).ToString();
            }
            catch (Exception e)
            {
                status.text = e.Message;
            }

            var json = PlayerPrefs.GetString(anchorKeeperID);
            if (!string.IsNullOrEmpty(json))
                JsonUtility.FromJsonOverwrite(json, savedAnchorNames);
            var names = savedAnchorNames.anchorNames;

            try
            {
                foreach (var name in names)
                {
                    var anchorId = anchorStore.LoadAnchor(name);
                    var anchor = anchorManager.GetAnchor(anchorId);
                    anchors.Add(anchor);
                    var visual = Instantiate(prefabForVisualization);
                    var anchorTr = anchor.transform;

                    visual.SetPositionAndRotation(anchorTr.position, anchorTr.rotation);
                    trackables.Add(visual);
                    visual.gameObject.SetActive(false);
                }
            }
            catch(Exception e)
            {
                status.text = "Could not load anchors despite them being there";
            }
        }

        public void Clear()
        {
            anchorStore?.Clear();

            foreach (var anchor in anchors)
                Destroy(anchor.gameObject);
            foreach (var visual in trackables)
                Destroy(visual.gameObject);

            anchors.Clear();
            trackables.Clear();
        }
        public void CreateNewAnchor()
        {
            if (anchorStore == null)
            {
                status.text = "Can't create. No store";
                return;
            }
            var visual = Instantiate(prefabForVisualization);
            var tr = visual.transform;
            tr.position = mainCamera.transform.position + new Vector3(0, 0, 0.3f);
            trackables.Add(visual);
        }
        public void OpenAnchorEdit()
        {
            foreach (var track in trackables)
                track.gameObject.SetActive(true);
        }
        public void ExitAnchorEdit()
        {
            foreach (var track in trackables)
                track.gameObject.SetActive(false);
        }
        public void Save()
        {
            if (anchorStore == null)
            {
                status.text = "No anchor store";
                return;
            }
            anchorStore?.Clear();
            int i = 0;
            var anchorNames = savedAnchorNames.anchorNames;
            anchorNames.Clear();

            //Destroy previous anchors
            foreach (var anchor in Anchors)
                Destroy(anchor);
            anchors.Clear();

            //Recreate them from trackables
            foreach (var track in trackables)
            {
                var name = getAnchorName(i);
                anchorNames.Add(name);
                var go = new GameObject(name);
                var tr = go.transform;
                var anchor = go.AddComponent<ARAnchor>();
                tr.SetPositionAndRotation(track.position, track.rotation);
                anchorStore?.TryPersistAnchor(anchor.trackableId, name);
                i++;
            }

            var json = JsonUtility.ToJson(savedAnchorNames);
            PlayerPrefs.SetString(anchorKeeperID, json);
        }
        private string getAnchorName(int i) => $"anchor_{i}";

        [Serializable]
        private class SavedAnchors
        {
            public List<string> anchorNames = new List<string>();
        }
    }
}