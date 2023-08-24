using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragVisual : MonoBehaviour
{
    public RectTransform imageContainer;
    public Image spriteRenderer;
    private Animator _animator;

    private Animator animator => _animator ?? (_animator = GetComponent<Animator>());
    private Vector3 velocity;
    private Vector3 targetVelocity;
    private float transitionSpeed = 0.1f;

    void Update()
    {
        Vector3 currentPosition = imageContainer.position;
        targetVelocity *= 0.999f;

        velocity = Vector3.Lerp(velocity, targetVelocity, transitionSpeed);

        animator.SetFloat("VelocityX", velocity.x / 10);
        animator.SetFloat("VelocityY", velocity.y / 10);
    }

    public void SetTargetVelocity(Vector3 newTargetVelocity)
    {
        targetVelocity = newTargetVelocity;
    }

    public void SnapTo(Vector3 position)
    {
        imageContainer.position = position;
    }

    public void MoveTo(Vector3 position)
    {
        SetTargetVelocity(imageContainer.position - position);
        imageContainer.position = position;
    }

    public void DragStarted()
    {
        animator.SetBool("Dragging", true);
    }

    public void DragStopped()
    {
        animator.SetBool("Dragging", false);
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
