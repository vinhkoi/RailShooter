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

        float lastMoveTime;
        float lastMoveDirection;

        public event Action LeftTag;
        public event Action RightTag;


        public Vector2 Move => moveAction.ReadValue<Vector2>();

        
        void Awake()
        {
            
            moveAction = playerInput.actions["Move"];
        }

        void OnEnable()
        {
            moveAction.performed += OnMovePerformed;
        }

        void OnDisEnable()
        {
            moveAction.performed -= OnMovePerformed;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
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
