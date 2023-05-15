using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public class SolarPower : MonoBehaviour
    {
        [SerializeField] private PowerGridComponent _powerGridComponent;

        public float PowerMultiplier = 1f;

        private void Awake()
        {
            _powerGridComponent.basePowerDraw *= PowerMultiplier;
        }
    }
}