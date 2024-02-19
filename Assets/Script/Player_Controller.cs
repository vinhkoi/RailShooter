using DG.Tweening;
using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;


namespace RailShooter
{

    public class PlayerController : ValidatedMonoBehaviour
    {
        [SerializeField, Self] InputReader input;

        [SerializeField] Transform followTarget;
        [SerializeField] Transform aimTarget;

        [SerializeField] Transform playerModel;
        [SerializeField] float followDistance = 2f;
        [SerializeField] Vector2 movementLimit = new Vector2 (2f, 2f);
        [SerializeField] float MovementSpeed = 10f;

        [SerializeField] float smoothTime = 0.2f;

        [SerializeField] float maxRoll = 15f;
        [SerializeField] float rollSpeed = 10f;
        [SerializeField] float rollDuration = 1f;

        [SerializeField] Transform modelParent;
        [SerializeField] float rotationSpeed = 5f;
        Vector3 velocity;

        float roll;

        void Awake()
        {
            input.LeftTag += OnLeftTag;
            input.RightTag += OnRightTag;

        }


        void Update()
        {
            HandelPosition();
            HandelRoll();
            HandelRotation();
        }

        void HandelRotation()
        {
            Vector3 direction = aimTarget.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            modelParent.rotation = Quaternion.Lerp(modelParent.rotation, targetRotation, Time.deltaTime*rotationSpeed);

        }

        void HandelPosition()
        {
            Vector3 targetPos = followTarget.position + followTarget.forward * -followDistance;

            Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

            Vector3 localPos = transform.InverseTransformPoint(smoothedPos);
            localPos.x += input.Move.x * MovementSpeed + Time.deltaTime;
            localPos.y += input.Move.y * MovementSpeed + Time.deltaTime;

            localPos.x = Mathf.Clamp(localPos.x, -movementLimit.x, movementLimit.x);
            localPos.y = Mathf.Clamp(localPos.y, -movementLimit.y, movementLimit.y);

            transform.position = transform.TransformPoint(localPos);
        }

        void HandelRoll()
        {
            transform.rotation = followTarget.rotation;

            roll = Mathf.Lerp(roll, input.Move.x * maxRoll, Time.deltaTime * rollSpeed);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, roll);
        }


        void OnRightTag() => BarrelRoll(1);
        void OnLeftTag() => BarrelRoll();


        void BarrelRoll(int direction = -1)
        {
            if (!DOTween.IsTweening(playerModel))
            {
                playerModel.DOLocalRotate(
                    new Vector3(
                playerModel.localEulerAngles.x,
                playerModel.localEulerAngles.y,
                360 * direction), rollDuration, RotateMode.LocalAxisAdd).SetEase(Ease.OutCubic);
            }
        }
        void OnDestroy()
        {
            input.LeftTag -= OnLeftTag;
            input.RightTag -= OnRightTag;
        }
    }
}
