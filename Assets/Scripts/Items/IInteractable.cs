using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public interface IInteractable
    {
        /// <summary>
        /// インタラクト
        /// </summary>
        void Interacted();
    }
}