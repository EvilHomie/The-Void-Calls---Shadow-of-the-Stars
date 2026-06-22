//using DI;
//using GameCamera;
//using System;
//using UnityEngine;

//public class LeadMarkerDebug : MonoBehaviour
//{
//    [SerializeField] Rigidbody2D targetRb;
//    [SerializeField] Rigidbody2D playerRb;
//    [SerializeField] WeaponData[] weapons;
//    [SerializeField] Transform LeadMarkerTransform;

//    private MouseCursor _mouseCursor;


//    [Serializable]
//    public class WeaponData
//    {
//        public Transform ShootPoint;
//        public float ProjectileSpeed = 5f;
//        public float MaxDistance = 10f;
//        public Transform AimPoint;
//    }

//    [Inject]
//    public void Construct(MouseCursor mouseCursor)
//    {
//        _mouseCursor = mouseCursor;
//    }

//    private void Update()
//    {
//        var targetVelocity = targetRb.linearVelocity;
//        var playerVelocity = playerRb.linearVelocity;
//        var velocityDifference = targetVelocity - playerVelocity;

//        //foreach (var weapon in weapons)
//        //{

//        //}
//        SetLeadMarker(weapons[0]);

//        //LeadMarkerTransform.position = GetInterceptPoint(weapons[0]);

//        foreach (var weapon in weapons)
//        {
//            var shootTransform = weapon.ShootPoint;
//            var shootPosition = shootTransform.position;
//            var distanceToCursor = Vector2.Distance(_mouseCursor.WorldPostition, shootPosition);
//            weapon.AimPoint.position = shootPosition + shootTransform.up * Mathf.Min(weapon.MaxDistance, distanceToCursor);
//        }
//    }

//    private void SetLeadMarker(WeaponData weaponData)
//    {
//        var shootSpotTransform = weaponData.ShootPoint;
//        Vector2 weaponDirection = shootSpotTransform.up;
//        var shipVelocity = playerRb.linearVelocity;
//        var targetVelocity = targetRb.linearVelocity;
//        Vector2 targetPosition = targetRb.transform.position;
//        Vector2 weaponPosition = shootSpotTransform.position;

//        var boltVelocity = shipVelocity + weaponDirection * weaponData.ProjectileSpeed;
//        var boltRelativeVelocity = boltVelocity - targetVelocity;

//        if (boltRelativeVelocity.sqrMagnitude < 0.001f)
//        {
//            LeadMarkerTransform.position = targetPosition;
//            return;
//        }

//        var targetRelativeVelocity = targetVelocity - shipVelocity;
//        var toTarget = targetPosition - weaponPosition;

//        var timeToReach = toTarget.magnitude / boltRelativeVelocity.magnitude;
//        var interceptPoint = targetPosition + targetRelativeVelocity * timeToReach;

//        LeadMarkerTransform.position = interceptPoint;
//    }
//}