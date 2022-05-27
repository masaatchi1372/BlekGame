using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelTutorialAnimation : MonoBehaviour
{
    public int tutorialThreshold = 50;
    private Animator animator;
    private int dragCount = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            dragCount++;
            animator.enabled = false;
        }

        if (dragCount < tutorialThreshold)
        {
            // we should pause the drag animation and also destroy the object
            animator.Play("DragTutorial");
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
