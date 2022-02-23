using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Players {
    public class PlayerInputController : MonoBehaviour, IInputEventProvider
    {
        public Vector2 Move { get; set; } = Vector2.zero;

        public bool Run { get; set; } = false;

        public bool Crouch { get; set; } = false;

        public bool Interact { get; set; } = false;

        void Start()
        {

        }

        void Update()
        {
            
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.started) Run = true;
            if (context.canceled) Run = false;
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.started) Crouch = true;
            if (context.canceled) Crouch = false;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started) Interact = true;
            if (context.canceled) Interact = false;
        }
    }
}