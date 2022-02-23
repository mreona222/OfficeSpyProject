using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Players;

namespace Items 
{
    public class InteractItem : MonoBehaviour, IInteractable
    {
        void Start()
        {

        }

        void Update()
        {

        }

        public void Interacted()
        {
            Destroy(gameObject);
        }
    }
}