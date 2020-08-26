using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LWFramework.FMS 
{
    public class FSMTypeAttribute : Attribute
    {
        public string FSMName;
        public bool isFirst;
        public FSMTypeAttribute(string FSMName, bool isFirst)
        {
            this.FSMName = FSMName;
            this.isFirst = isFirst;
        }
    }
}


