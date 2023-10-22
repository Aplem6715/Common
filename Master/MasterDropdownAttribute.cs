using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLogger;

namespace Aplem.Common
{

    public class MasterDropdownAttribute : ValueDropdownAttribute
    {
        public Type MasterType;
        public string Folder;

        public MasterDropdownAttribute(Type masterType, string folder = "Assets/Project/Master") : base("")
        {
            MasterType = masterType;
            Folder = folder;
        }
    }

}