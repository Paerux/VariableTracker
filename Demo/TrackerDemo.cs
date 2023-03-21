using System;
using UnityEngine;

namespace _Project.Scripts.VariableTracker.Demo
{
    public class TrackerDemo : MonoBehaviour
    {
        [Track("Field 1")]
        private int trackField1;
        [Track("Field 2")]
        private int trackField2;
        private int trackField3;
        private int trackField4;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                trackField1++;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                trackField2++;
            if (Input.GetKeyDown(KeyCode.Alpha3))
                trackField3++;
            if (Input.GetKeyDown(KeyCode.Alpha4))
                trackField4++;
        }
    }
}