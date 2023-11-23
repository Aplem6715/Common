using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aplem.Common;


namespace Aplem.Common
{
    public class PoolTestObj : MonoBehaviour, IPoolableMono
    {
        public bool IsPooling { get; set; }
        public IPool _pool { get; set; }

        public void OnRent()
        {
        }

        public void OnReturned()
        {
        }
    }
}
