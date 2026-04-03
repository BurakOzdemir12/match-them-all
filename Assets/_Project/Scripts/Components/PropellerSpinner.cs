using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Components
{
    public class PropellerSpinner : MonoBehaviour
    {
        [Header("References")] [Tooltip("Spin Object Reference")] [SerializeField]
        private List<Transform> spinObjectList;

        [SerializeField] private float spinSpeed;

        [Tooltip("Which axis to spin on? (X=1,0,0 | Y=0,1,0 | Z=0,0,1)")] [SerializeField]
        private Vector3 spinAxis = Vector3.up;

        private void Update()
        {
            if (spinObjectList == null || spinObjectList.Count == 0) return;
            Vector3 frameRotation = spinAxis.normalized * (spinSpeed * Time.deltaTime);

            foreach (var obj in spinObjectList)
            {
                if (obj != null)
                {
                    obj.Rotate(frameRotation);
                }
            }
        }
    }
}