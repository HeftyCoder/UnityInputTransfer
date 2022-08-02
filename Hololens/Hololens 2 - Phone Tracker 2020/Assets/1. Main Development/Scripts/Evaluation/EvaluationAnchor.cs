using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace UOPHololens.Evaluation {
    [RequireComponent(typeof(ARAnchor), typeof(Collider), typeof(Renderer))]
    public class EvaluationAnchor : MonoBehaviour
    {
        Collider col;
        Renderer rend;
        
        private void Awake()
        {
            col = GetComponent<Collider>();
            rend = GetComponent<Renderer>();
        }

        public void OpenEdit()
        {
            col.enabled = true;
            rend.enabled = true;
        }
        public void CloseEdit()
        {
            col.enabled = false;
            rend.enabled = false;
        }
    }
}


