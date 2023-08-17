using System;
using UnityEngine;

public partial class UIManager
{
    [Serializable]
    public struct StateImageDependency
    {
        public Sprite sprite;
        public GameController.State state;
    }
}
