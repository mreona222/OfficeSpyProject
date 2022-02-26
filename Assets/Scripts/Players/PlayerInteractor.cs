using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Items;

namespace Players
{
    public class PlayerInteractor : MonoBehaviour
    {
        IInputEventProvider _playerInput;

        Slider _interactGauge;

        float _interactTimer = 0f;

        void Start()
        {
            _playerInput = GetComponent<IInputEventProvider>();
            _interactGauge = GameObject.Find("Canvas").transform.Find("InteractText/InteractGauge").GetComponent<Slider>();
        }

        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<IInteractable>() != null)
            {
                GameObject.Find("Canvas").transform.Find("InteractText").gameObject.SetActive(true);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<IInteractable>() != null)
            {
                if (_playerInput.Interact)
                {
                    _interactTimer += Time.deltaTime;
                    _interactGauge.value = _interactTimer;
                    if (_interactTimer > 2.0f)
                    {
                        other.GetComponent<IInteractable>().Interacted();
                    }
                }
                else
                {
                    _interactTimer = 0f;
                    _interactGauge.value = _interactTimer;
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<IInteractable>() != null)
            {
                GameObject.Find("Canvas").transform.Find("InteractText").gameObject.SetActive(false);
            }
        }
    }
}