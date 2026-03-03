using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.UI
{
    public class UICanvasView : MonoBehaviour
    {
       public List<UIWindow> m_ListofWindows = new List<UIWindow>();

        public void Reset()
        {
            m_ListofWindows = GetComponentsInChildren<UIWindow>().ToList();
        }
    }
}
