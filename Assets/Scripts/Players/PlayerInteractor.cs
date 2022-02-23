using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Items;

namespace Players
{
    public class PlayerInteractor : MonoBehaviour
    {
        IInputEventProvider _playerInput;

        float _interactTimer = 0f;

        void Start()
        {
            _playerInput = GetComponent<IInputEventProvider>();
        }

        void Update()
        {

        }

        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<IInteractable>() != null)
            {
                if (_playerInput.Interact)
                {
                    _interactTimer += Time.deltaTime;
                    if (_interactTimer > 2.0f)
                    {
                        other.GetComponent<IInteractable>().Interacted();
                    }
                }
                else
                {
                    _interactTimer = 0f;
                }
            }
        }
    }
}