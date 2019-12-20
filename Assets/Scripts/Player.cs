using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody RB;
    public float MoveSpeed = 5;
    public bool Won;

    private Vector3 _input;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // Detect user input and apply its value times moveSpeed, on the RigidBody's velocity.
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        RB.velocity = _input * MoveSpeed;
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // If Player touches Pallet, Player wins.
        if (collision.gameObject.CompareTag("Pellet"))
            Won = true;
    }
}
