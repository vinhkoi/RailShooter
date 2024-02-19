using KBCore.Refs;
using UnityEngine;

namespace RailShooter
{
    public class WeaponSystem : ValidatedMonoBehaviour
    {
        [SerializeField, Self] InputReader input;
        [SerializeField] Transform TargetPoint;
        [SerializeField] float TargetDistance = 50f;
        [SerializeField] float smoothTime = 0.2f;
        [SerializeField] Vector2 aimLimit = new Vector2(50f, 20f);
        [SerializeField] float AimSpeed = 10f;
        [SerializeField] float AimReturnSpeed = 0.2f;

        [SerializeField] GameObject projectilePrefab;
        [SerializeField] Transform firePoint;

        Vector3 velocity;
        Vector2 aimOffset;

        void Awake()
        {
            input.Fire += OnFire;
        }

        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;


        }
        void Update()
        {
            Vector3 targetPosition = transform.position+transform.forward*TargetDistance;
            Vector3 localPos = transform.InverseTransformDirection(targetPosition);

            if(input.Aim != Vector2.zero)
            {
                aimOffset += input.Aim * AimSpeed * Time.deltaTime;
                aimOffset.x = Mathf.Clamp(aimOffset.x, -aimLimit.x, aimLimit.x);
                aimOffset.y = Mathf.Clamp(aimOffset.y, -aimLimit.y, aimLimit.y);


            }
            else
            {
                aimOffset = Vector2.Lerp(aimOffset,Vector2.zero, Time.deltaTime*AimReturnSpeed);
            }

            localPos.x = aimOffset.x;
            localPos.y = aimOffset.y;

            var desirePosition = transform.TransformPoint(localPos);
            TargetPoint.position = Vector3.SmoothDamp(TargetPoint.position,desirePosition,ref velocity,smoothTime);
        }
        void OnFire()
        {
            var direction = TargetPoint.position - firePoint.position;
            var projectile = Instantiate(projectilePrefab,firePoint.position,Quaternion.LookRotation(TargetPoint.position-firePoint.position));
            Destroy(projectile,5f);
        }
        void OnDestroy()
        {
            input.Fire -= OnFire;
        }
    }
}
