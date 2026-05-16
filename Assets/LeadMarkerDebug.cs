using System;
using UnityEngine;

public class LeadMarkerDebug : MonoBehaviour
{
    [SerializeField] Rigidbody2D targetRb;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] WeaponData[] weapons;
    

    [Serializable]
    public class WeaponData
    {
        public Transform ShootPoint;
        public float ProjectileSpeed = 5f;
        public Transform LeadMarkerTransform;
    }

    private void Update()
    {
        var targetVelocity = targetRb.linearVelocity;
        var playerVelocity = playerRb.linearVelocity;
        var velocityDifference = targetVelocity - playerVelocity;        

        foreach (var weapon in weapons)
        {
            SetLeadMarkers(weapon, velocityDifference);
        }
    }

    private void SetLeadMarkers(WeaponData  weaponData, Vector3 velocityDifference)
    {
        var shootSpotTransform = weaponData.ShootPoint;
        Vector2 direction = shootSpotTransform.up;
        var shipVelocity = playerRb.linearVelocity;
        var targetPosition = targetRb.transform.position;
        var weaponPosition = shootSpotTransform.position;
        var boltVelocity = shipVelocity + direction * weaponData.ProjectileSpeed;

        var distance = Vector2.Distance(targetPosition, weaponPosition);
        float speed = boltVelocity.magnitude;

        float timeToTarget = distance / speed;
        var leadPosition = targetPosition + timeToTarget * velocityDifference;
        weaponData.LeadMarkerTransform.position = leadPosition;
    }
}