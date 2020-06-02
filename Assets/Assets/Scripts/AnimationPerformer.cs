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

    private float lightAttackSlideSpeed = 5f;
    private float heavyAttackSlideSpeed = 8f;
    private float returnSlideSpeed = 8f;
    private float healSpeed = 1f;

    private float slideSpeed = 2f;
    private float reachedDistance = 0.05f;
    
    public void LightAttack(Unit attacker, Unit target, Action onSlideComplete, Action onAttackComplete)
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 attackerPosition = attacker.transform.position;

        Vector3 slideTargetPosition = targetPosition + (attackerPosition - targetPosition).normalized * 2f;
        Vector3 startingPosition = attackerPosition;

        this.attacker = attacker;

        // slide to target
        SlideToPosition(slideTargetPosition, lightAttackSlideSpeed, () =>
        {
            // arrived at target
            //Vector3 attackDir = (target.GetPosition() - GetPosition()).normalized;
            onSlideComplete();
            attacker.transform.position = targetPosition + new Vector3(3f, 3f, 1f);

            SlideToPosition(targetPosition + new Vector3(-3f, -3f, 1f), heavyAttackSlideSpeed, () =>
            {
                attacker.transform.position = targetPosition;
                // attack completed, slide back
                SlideToPosition(startingPosition, returnSlideSpeed, () =>
                {
                    // slide completed back to idle
                    onAttackComplete();
                });
            });

            
        });
    }
    
    public void HeavyAttack(Unit attacker, Unit target, Action onSlideComplete, Action onAttackComplete)
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 attackerPosition = attacker.transform.position;

        Vector3 slideTargetPosition = targetPosition + (new Vector3(0, 3f));
        Vector3 slideHeavyAttackPosition = targetPosition - (new Vector3(0, 2f));
        Vector3 startingPosition = attackerPosition;

        this.attacker = attacker;

        // slide to target
        SlideToPosition(slideTargetPosition, lightAttackSlideSpeed, () =>
        {
            // arrived at target
            //Vector3 attackDir = (target.GetPosition() - GetPosition()).normalized;
            onSlideComplete();
            
            // attack downwards
            SlideToPosition(slideHeavyAttackPosition, heavyAttackSlideSpeed, () =>
            {
                // attack complete
                SlideToPosition(startingPosition, returnSlideSpeed, () =>
                {
                    // slide completed back to idle
                    onAttackComplete();
                });
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

    private void SlideToPosition(Vector3 slideTargetPosition,float speed, Action onSlideComplete)
    {
        this.slideTargetPosition = slideTargetPosition;
        this.onSlideComplete = onSlideComplete; 
        slideSpeed = speed;
        finished = false;
    }
}