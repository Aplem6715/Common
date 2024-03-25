using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLogger;
using Aplem.Common;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace Aplem.Common.UI
{
    public interface IView
    {
        public void Show();
        public void Hide();
    }
}