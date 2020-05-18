using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AnimationPerformer : MonoBehaviour
{
    private Boolean finished = true;
    private Vector3 slideTargetPosition;
    private Action onSlideComplete;
    private Unit attacker;

    private float slideSpeed = 2f;
    private float reachedDistance = 0.25f;
    
    public void Attack(Unit attacker, Unit target, Action onSlideComplete, Action onAttackComplete)
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 attackerPosition = attacker.transform.position;

        Vector3 slideTargetPosition = targetPosition + (attackerPosition - targetPosition).normalized * slideSpeed;
        Vector3 startingPosition = attackerPosition;

        this.attacker = attacker;

        // slide to target
        SlideToPosition(slideTargetPosition, () =>
        {
            // arrived at target
            //Vector3 attackDir = (target.GetPosition() - GetPosition()).normalized;
            onSlideComplete();
            
            // attack completed, slide back
            SlideToPosition(startingPosition, () =>
            {
                // slide completed back to idle
                onAttackComplete();
            });
        });
    }


    private void Update()
    {
        if (!finished)
        {
            attacker.transform.position += (slideTargetPosition - attacker.transform.position) * (slideSpeed * Time.deltaTime);

            if (Vector3.Distance(attacker.transform.position, slideTargetPosition) < reachedDistance)
            {
                attacker.transform.position = slideTargetPosition;
                finished = true;
                onSlideComplete();
            }
        }
    }

    private void SlideToPosition(Vector3 slideTargetPosition, Action onSlideComplete)
    {
        this.slideTargetPosition = slideTargetPosition;
        this.onSlideComplete = onSlideComplete;
        finished = false;
    }
}