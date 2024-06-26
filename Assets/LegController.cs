using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LegController : MonoBehaviour
{
    public Transform[] legTargets;
    public Transform[] legCubes;
    public Transform tank;
    Vector3[] legPositions;
    Vector3[] legOriginalPositions;
    Vector3 velocity;
    Vector3 lastTankPosition;
    Vector3 lastVelocity;

    [SerializeField] float moveDistance = 1;
    [SerializeField] int legMoveSmoothness = 5;
    [SerializeField] int velocitySmooth = 3;
    [SerializeField] float overStepMultiplier = 1.3f;
    public int waitTimeBetweenSteps = 2;
    public float tankJitterCutoff = 0.1f;
    public float stepHeight = 0.5f;

    List<int> oppositeLegIndex = new List<int>();
    List<int> nextIndexToMove = new List<int>();
    List<int> indexMoving = new List<int>();

    bool currentLeg = true;

    // Start is called before the first frame update
    void Start()
    {
        lastTankPosition = tank.position;

        legPositions = new Vector3[legTargets.Length];
        legOriginalPositions = new Vector3[legTargets.Length];

        for(int i = 0; i < legTargets.Length; i++)
        {
            legPositions[i] = legTargets[i].position;
            legOriginalPositions[i] = legPositions[i];

            if (currentLeg)
            {
                oppositeLegIndex.Add(i + 1);
                currentLeg = false;
            }

            else if (!currentLeg) { oppositeLegIndex.Add(i - 1); currentLeg = true; }
        }
    }

    // Update is called once per frame
    void Update()
    {
        velocity  = tank.position - lastTankPosition;
        velocity = velocity + velocitySmooth * lastVelocity;
        velocity = velocity / (velocitySmooth + 1);

        MoveLegs();

        lastTankPosition = tank.position;
        lastVelocity = velocity;
    }

    void MoveLegs()
    {
        for(int i = 0; i < legTargets.Length; i++)
        {
            if (Vector3.Distance(legTargets[i].position, legCubes[i].position) >= moveDistance)
            {
                if(!nextIndexToMove.Contains(i) && !indexMoving.Contains(i)) nextIndexToMove.Add(i);
                
            }

            else if (!indexMoving.Contains(i))
            {
                legTargets[i].position = legOriginalPositions[i];
            }
            
        }

        if (nextIndexToMove.Count == 0 || indexMoving.Count != 0) return;

        Vector3 targetPosition = legCubes[nextIndexToMove[0]].position;
        targetPosition = targetPosition + Mathf.Clamp(velocity.magnitude * overStepMultiplier, 0, 2) * (legCubes[nextIndexToMove[0]].position - legTargets[nextIndexToMove[0]].position) + velocity * overStepMultiplier;
        StartCoroutine(Step(nextIndexToMove[0], targetPosition, false)) ;
        
    }

    IEnumerator Step(int index, Vector3 moveTo, bool isOpposite)
    {
        if(!isOpposite) { MoveOppositeLeg(oppositeLegIndex[index]); }

        if (nextIndexToMove.Contains(index)) nextIndexToMove.Remove(index);
        if (!indexMoving.Contains(index)) indexMoving.Add(index);

        Vector3 startingPosition = legOriginalPositions[index];

        for(int i = 1; i <= legMoveSmoothness; i++)
        {
            legTargets[index].position = Vector3.Lerp(startingPosition, moveTo + new Vector3(0,Mathf.Sign(i / (legMoveSmoothness * tankJitterCutoff) * Mathf.PI) * stepHeight,0), i/legMoveSmoothness);
            yield return new WaitForFixedUpdate();
        }

        legOriginalPositions[index] = moveTo;

        for(int i = 1; i<= waitTimeBetweenSteps; i++) yield return new WaitForFixedUpdate();

        if (indexMoving.Contains(index)) indexMoving.Remove(index);

    }

    void MoveOppositeLeg(int index)
    {
        Vector3 targetPosition = legCubes[index].position;
        targetPosition = targetPosition + Mathf.Clamp(velocity.magnitude * overStepMultiplier, 0, 2) * (legCubes[index].position - legTargets[index].position) + velocity * overStepMultiplier;
        StartCoroutine(Step(index, targetPosition, true));

    }
}
