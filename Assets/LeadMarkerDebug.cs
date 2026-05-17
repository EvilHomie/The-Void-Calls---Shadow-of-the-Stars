using DI;
using GameCamera;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static LeadMarkerDebug;

public class LeadMarkerDebug : MonoBehaviour
{
    [SerializeField] Rigidbody2D targetRb;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] WeaponData[] weapons;
    [SerializeField] Transform LeadMarkerTransform;

    private MouseCursor _mouseCursor;


    [Serializable]
    public class WeaponData
    {
        public Transform ShootPoint;
        public float ProjectileSpeed = 5f;
        public float MaxDistance = 10f;
        public Transform AimPoint;
    }

    [Inject]
    public void Construct(MouseCursor mouseCursor)
    {
        _mouseCursor = mouseCursor;
    }

    private void Update()
    {
        var targetVelocity = targetRb.linearVelocity;
        var playerVelocity = playerRb.linearVelocity;
        var velocityDifference = targetVelocity - playerVelocity;

        //foreach (var weapon in weapons)
        //{

        //}
        SetLeadMarker(weapons[0]);

        //LeadMarkerTransform.position = GetInterceptPoint(weapons[0]);

        foreach (var weapon in weapons)
        {
            var shootTransform = weapon.ShootPoint;
            var shootPosition = shootTransform.position;
            var distanceToCursor = Vector2.Distance(_mouseCursor.WorldPostition, shootPosition);
            weapon.AimPoint.position = shootPosition + shootTransform.up * Mathf.Min(weapon.MaxDistance, distanceToCursor);
        }
    }

    private void FixedUpdate()
    {

    }


    //private void SetLeadMarker(WeaponData weaponData)
    //{
    //    var shootSpotTransform = weaponData.ShootPoint;
    //    Vector2 weaponDirection = shootSpotTransform.up;
    //    var shipVelocity = playerRb.linearVelocity;
    //    var targetVelocity = targetRb.linearVelocity;

    //    var boltVelocity = shipVelocity + weaponDirection * weaponData.ProjectileSpeed;
    //    var boltRelativeVelocity = boltVelocity - targetVelocity;

    //    Vector2 targetPosition = targetRb.transform.position;
    //    Vector2 weaponPosition = shootSpotTransform.position;

    //    var distance = Vector2.Distance(targetPosition, weaponPosition);
    //    float boltClosingSpeed = Vector2.Dot(boltRelativeVelocity, weaponDirection);
    //    float timeToTarget = distance / boltClosingSpeed;

    //    var targetRelativeVelocity = targetVelocity - boltVelocity;


    //    var leadPosition = targetPosition + timeToTarget * targetRelativeVelocity;
    //    LeadMarkerTransform.position = leadPosition;
    //}

    private void SetLeadMarker(WeaponData weaponData)
    {
        var shootSpotTransform = weaponData.ShootPoint;
        Vector2 weaponDirection = shootSpotTransform.up;
        var shipVelocity = playerRb.linearVelocity;
        var targetVelocity = targetRb.linearVelocity;
        Vector2 targetPosition = targetRb.transform.position;
        Vector2 weaponPosition = shootSpotTransform.position;

        var boltVelocity = shipVelocity + weaponDirection * weaponData.ProjectileSpeed;
        var boltRelativeVelocity = boltVelocity - targetVelocity;

        var targetRelativeVelocity = targetVelocity - shipVelocity;

        float boltClosingSpeed = Vector2.Dot(boltRelativeVelocity, (targetPosition - weaponPosition).normalized);
        float distance = Vector2.Distance(targetPosition, weaponPosition);

        var toTarget = targetPosition - weaponPosition;

        float timeToReach = toTarget.magnitude / boltRelativeVelocity.magnitude;
        Vector2 interceptPoint = targetPosition + targetRelativeVelocity * timeToReach;


        LeadMarkerTransform.position = interceptPoint;
    }

    //public Vector2 GetInterceptPoint(WeaponData weaponData)
    //{
    //    var shootSpotTransform = weaponData.ShootPoint;
    //    Vector2 weaponDirection = shootSpotTransform.up;
    //    Vector2 shooterPosition = shootSpotTransform.position;
    //    Vector2 targetPosition = targetRb.transform.position;
    //    Vector2 targetVelocity = targetRb.linearVelocity;
    //    Vector2 projectileVelocity = playerRb.linearVelocity + weaponDirection * weaponData.ProjectileSpeed;

    //    // 1. Получаем чистую скорость снаряда (его модуль)
    //    float projectileSpeed = projectileVelocity.magnitude;

    //    // 2. Вектор между стрелком и целью в данный момент
    //    Vector2 targetDir = targetPosition - shooterPosition;
    //    float distance = targetDir.magnitude;

    //    // 3. Составляем коэффициенты квадратного уравнения: a*t^2 + b*t + c = 0
    //    // Учитываем относительную скорость цели
    //    float a = targetVelocity.sqrMagnitude - (projectileSpeed * projectileSpeed);
    //    float b = 2f * Vector2.Dot(targetDir, targetVelocity);
    //    float c = targetDir.sqrMagnitude;

    //    // 4. Считаем дискриминант
    //    float discriminant = (b * b) - (4f * a * c);

    //    // Если дискриминант отрицательный, перехват физически невозможен
    //    if (discriminant < 0)
    //    {
    //        // Возвращаем текущую позицию цели как запасной вариант
    //        return targetPosition;
    //    }

    //    // 5. Находим корни уравнения (время t)
    //    float t;
    //    float t1 = (-b + Mathf.Sqrt(discriminant)) / (2f * a);
    //    float t2 = (-b - Mathf.Sqrt(discriminant)) / (2f * a);

    //    // Нам нужно минимальное положительное время
    //    if (t1 > 0 && t2 > 0) t = Mathf.Min(t1, t2);
    //    else if (t1 > 0) t = t1;
    //    else if (t2 > 0) t = t2;
    //    else return targetPosition; // Время отрицательное (цель улетает слишком быстро)

    //    // 6. Вычисляем итоговую точку перехвата
    //    Vector2 interceptPoint = targetPosition + (targetVelocity * t);
    //    return interceptPoint;
        
    //}



    //private void SetLeadMarker(WeaponData weaponData)
    //{
    //    var shootSpotTransform = weaponData.ShootPoint;

    //    Vector2 weaponDirection = shootSpotTransform.up;

    //    Vector2 shipVelocity = playerRb.linearVelocity;
    //    Vector2 targetVelocity = targetRb.linearVelocity;

    //    Vector2 boltVelocity = shipVelocity + weaponDirection * weaponData.ProjectileSpeed;

    //    Vector2 relativeVelocity = targetVelocity - boltVelocity;

    //    Vector2 targetPosition = targetRb.position;
    //    Vector2 weaponPosition = shootSpotTransform.position;

    //    float distance = Vector2.Distance(targetPosition, weaponPosition);

    //    //float closingSpeed = -Vector2.Dot(relativeVelocity, (targetPosition - weaponPosition).normalized);
    //    float closingSpeed = Vector2.Dot(boltVelocity - targetVelocity, (targetPosition - weaponPosition).normalized);

    //    //if (closingSpeed <= 0f)
    //    //    return;

    //    float timeToTarget = distance / closingSpeed;

    //    Vector2 leadPosition = targetPosition + relativeVelocity * timeToTarget;

    //    LeadMarkerTransform.position = leadPosition;
    //}

    //private void SetLeadMarker(WeaponData weaponData)
    //{
    //    var shootSpotTransform = weaponData.ShootPoint;

    //    Vector2 weaponDirection = shootSpotTransform.up;

    //    Vector2 shipVelocity = playerRb.linearVelocity;
    //    Vector2 targetVelocity = targetRb.linearVelocity;

    //    // Относительное движение цели
    //    Vector2 relativeVelocity = targetVelocity - shipVelocity;

    //    Vector2 targetPosition = targetRb.position;
    //    Vector2 weaponPosition = shootSpotTransform.position;

    //    float distance = Vector2.Distance(targetPosition, weaponPosition);

    //    Vector2 toTarget = (targetPosition - weaponPosition).normalized;

    //    // Скорость снаряда В СИСТЕМЕ ИГРОКА
    //    float closingSpeed = Vector2.Dot(weaponDirection * weaponData.ProjectileSpeed, toTarget);

    //    if (closingSpeed <= 0f) return;

    //    float timeToTarget = distance / closingSpeed;

    //    Vector2 leadPosition = targetPosition + relativeVelocity * timeToTarget;

    //    LeadMarkerTransform.position = leadPosition;
    //}

    //private void SetLeadMarker(WeaponData weaponData)
    //{
    //    Vector2 shooterPos = weaponData.ShootPoint.position;
    //    Vector2 targetPos = targetRb.position;

    //    Vector2 shooterVel = playerRb.linearVelocity;
    //    Vector2 targetVel = targetRb.linearVelocity;

    //    float projectileSpeed = weaponData.ProjectileSpeed;

    //    Vector2 relativePos = targetPos - shooterPos;
    //    Vector2 relativeVel = targetVel - shooterVel;

    //    float a = Vector2.Dot(relativeVel, relativeVel) - projectileSpeed * projectileSpeed;
    //    float b = 2f * Vector2.Dot(relativeVel, relativePos);
    //    float c = Vector2.Dot(relativePos, relativePos);

    //    float discriminant = b * b - 4f * a * c;

    //    if (discriminant < 0f)
    //    {
    //        // Перехват невозможен
    //        return;
    //    }

    //    float sqrt = Mathf.Sqrt(discriminant);

    //    float t1 = (-b - sqrt) / (2f * a);
    //    float t2 = (-b + sqrt) / (2f * a);

    //    float time = Mathf.Min(t1, t2);

    //    if (time < 0f)
    //        time = Mathf.Max(t1, t2);

    //    if (time < 0f)
    //    {
    //        // Обе точки в прошлом
    //        return;
    //    }

    //    Vector2 leadPosition = targetPos + relativeVel * time;

    //    LeadMarkerTransform.position = leadPosition;
    //}




    //private void SetLeadMarkers(WeaponData weaponData, Vector2 velocityDifference)
    //{
    //    var shootSpotTransform = weaponData.ShootPoint;
    //    Vector2 direction = shootSpotTransform.up;
    //    var shipVelocity = playerRb.linearVelocity;
    //    Vector2 targetPosition = targetRb.transform.position;
    //    Vector2 weaponPosition = shootSpotTransform.position;
    //    var boltVelocity = shipVelocity + direction * weaponData.ProjectileSpeed;

    //    var distance = Vector2.Distance(targetPosition, weaponPosition);


    //    Vector2 toTarget = (targetPosition - weaponPosition).normalized;
    //    float closingSpeed = Vector2.Dot(boltVelocity, toTarget);

    //    float timeToTarget = distance / closingSpeed;
    //    var leadPosition = targetPosition + timeToTarget * velocityDifference;
    //    LeadMarkerTransform.position = leadPosition;
    //}
}