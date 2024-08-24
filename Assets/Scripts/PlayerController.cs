using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject angle, anglebar;
    public UI ui;
    public Rigidbody rb;
    public Animator animator;
    public bool left, firstJumpStarted, firstJumpEnded, doubleJumped, failed;
    public float speed, jumpStartX;
    private Coroutine coroutine;

    private void Update()
    {
        animator.SetFloat("Speed", speed); // Sync animator speed with actual speed
        speed = Mathf.Max(0, speed - Time.deltaTime * 0.5f); // Reduce speed every frame

        // Move right at speed if we're not jumping yet
        if (!firstJumpStarted && !firstJumpEnded)
            transform.position += speed * Time.deltaTime * Vector3.right;


        if (failed)
        {
            // Reduce speed quicker if we've failed
            speed = Mathf.Max(0, speed - Time.deltaTime * 2f);
            return;
        }

        HandleInput();
    }

    private void HandleInput()
    {
        if (left && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            speed += 0.1f;
            left = !left;
            return;
        }
        
        if (!left && Input.GetKeyDown(KeyCode.RightArrow))
        {
            speed += 0.1f;
            left = !left;
            return;
        }

        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        if (!firstJumpStarted)
        {
            coroutine = StartCoroutine(HoldJump());
            return;
        }
        
        if (!doubleJumped && !firstJumpEnded && coroutine == null)
        {
            doubleJumped = true;
            coroutine = StartCoroutine(HoldJump());
            return;
        }
    }

    private IEnumerator HoldJump()
    {
        var time = 0f;
        rb.isKinematic = true;
        firstJumpStarted = true;
        animator.SetFloat("Speed", 0);
        EnableAngleBar();

        while (Input.GetKey(KeyCode.Space) && time < 1f)
        {
            // Rotates angle bar between 0-90 degrees depending on how long space is held
            anglebar.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Lerp(0, 1, time) * 90f));
            time += Time.deltaTime;
            yield return null;
        }

        angle.SetActive(false);
        animator.SetTrigger("Jump");

        rb.isKinematic = false;
        rb.AddForce(75f * speed * anglebar.transform.right);
        jumpStartX = transform.position.x;
        coroutine = null;

        yield break;
    }

    private void EnableAngleBar()
    {
        angle.SetActive(true);
        anglebar.transform.rotation = Quaternion.identity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ui.ShowEndScreen();        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (firstJumpStarted)
            return;

        failed = true;
        ui.ShowEndScreen();
    }

    // Animator event
    public void Land()
    {
        firstJumpEnded = true;
        rb.isKinematic = false;
    }
}