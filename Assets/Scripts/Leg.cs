using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    [SerializeField] List<Leg> neighbouringLegs;

    [SerializeField] Transform idealLegPos;
    [SerializeField] float maxDistance, distanceLimit;
    [SerializeField] float rayLength = 1;
    [SerializeField] float yOffset;
    [SerializeField] public float travelSpeed, travelTimeLimit;
    [SerializeField] AnimationCurve easingFunction = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] LayerMask ground;
    public bool moving;
    float maxTimer = 1;
    float timer;
    Vector3 originalPosition;
    Vector3 midPoint;
    Vector3 targetPosition;

    [SerializeField] float maxStepDistance;

    float endYOffset = 0;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        targetPosition = transform.position;
        midPoint = transform.transform.position;
    }

    Vector3 QaudraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, t);
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, idealLegPos.position + Vector3.up * endYOffset);
        if (CanMove())
        {

            if (distance > maxDistance)
            {
                timer = 0;
                moving = true;
                originalPosition = transform.position;
                if (distance > maxStepDistance)
                {
                    originalPosition = idealLegPos.position + (transform.position - idealLegPos.position).normalized * maxStepDistance;
                }
                targetPosition = idealLegPos.position + (idealLegPos.position - originalPosition) * .8f; //* (maxDistance * .5f);




                maxTimer = Vector3.Distance(originalPosition, targetPosition) / travelSpeed;
                if (maxTimer > travelTimeLimit)
                    maxTimer = travelTimeLimit;
                RaycastHit hit;
                if (Physics.Raycast(targetPosition + Vector3.up * (Mathf.Max(originalPosition.y, targetPosition.y) + rayLength / 2), Vector3.down, out hit, rayLength / 2, ground))
                {
                    targetPosition = hit.point;
                    //endYOffset = hit.point.y - idealLegPos.position.y;
                }

                endYOffset = targetPosition.y - idealLegPos.position.y;
                midPoint = Vector3.Lerp(originalPosition, targetPosition, .5f);
                midPoint.y = Mathf.Max(originalPosition.y, targetPosition.y) + yOffset;
            }


        }


        transform.position = QaudraticLerp(originalPosition, midPoint, targetPosition, easingFunction.Evaluate(Mathf.Clamp01(timer / maxTimer)));
        if (distance > distanceLimit)
        {
            //transform.position = (idealLegPos.position + Vector3.up * endYOffset - transform.position).normalized * distanceLimit;
        }
        if (timer >= maxTimer && moving)
        {
            moving = false;
        }
        timer += Time.deltaTime;
    }

    bool CanMove()
    {
        if (moving)
            return false;
        foreach (Leg leg in neighbouringLegs)
        {
            if (leg.moving)
                return false;
        }

        return true;

    }

    private void OnDrawGizmosSelected()
    {
        if (!moving)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(idealLegPos.position + Vector3.up * endYOffset, maxDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(idealLegPos.position + Vector3.up * endYOffset, distanceLimit);
        }
        else
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(originalPosition, .1f);
            Gizmos.DrawWireSphere(midPoint, .1f);
            Gizmos.DrawWireSphere(targetPosition, .1f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(originalPosition, midPoint);
            Gizmos.DrawLine(midPoint, targetPosition);
        }




    }
}