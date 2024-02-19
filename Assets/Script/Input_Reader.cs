using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace RailShooter
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputReader : ValidatedMonoBehaviour
    {
        [SerializeField,Self] PlayerInput playerInput;
        [SerializeField] float doubleTapTime = 0.5f;

        InputAction moveAction;
        InputAction aimAction;
        InputAction fireAction;

        float lastMoveTime;
        float lastMoveDirection;

        public event Action LeftTag;
        public event Action RightTag;
        public event Action Fire;


        public Vector2 Move => moveAction.ReadValue<Vector2>();
        public Vector2 Aim => aimAction.ReadValue<Vector2>();


        void Awake()
        {
            moveAction = playerInput.actions["Move"];
            aimAction = playerInput.actions["Aim"];
            fireAction = playerInput.actions["Fire"];

        }

        void OnEnable()
        {
            moveAction.performed += OnMovePerformed;
            fireAction.performed += OnFire;

        }


        void OnDisEnable()
        {
            moveAction.performed -= OnMovePerformed;
            fireAction.performed -= OnFire;

        }

        void OnFire(InputAction.CallbackContext context) => Fire?.Invoke();
        void OnMovePerformed(InputAction.CallbackContext context)
        {
            float currentDirection = Move.x;
            if(Time.time - lastMoveTime < doubleTapTime && currentDirection == lastMoveDirection) 
            {
                if(currentDirection < 0)
                {
                    LeftTag?.Invoke();
                }
                else if(currentDirection > 0)
                {
                    RightTag?.Invoke();
                }
            }
            lastMoveTime = Time.time;
            lastMoveDirection = currentDirection;
        }
    }
}
