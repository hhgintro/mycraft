using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public interface IInput
    {
        public void TakeInput(Item item);
        public bool CanTakeInput(Item item);
    }
}