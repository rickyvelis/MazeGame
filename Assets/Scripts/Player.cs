using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed = 5;
    public bool waiting = true;

    private Vector3 _input;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Respawn(int x, int y)
    {
        waiting = false;
        gameObject.transform.position = new Vector3(x, 0, y);
    }

    public void Wait()
    {
        gameObject.transform.position = new Vector3(0f, -1f, 0f);
        waiting = true;
    }

    void Update()
    {
        //transform.Translate(
        //    Input.GetAxisRaw("Horizontal") * Time.fixedDeltaTime * moveSpeed,
        //    0,
        //    Input.GetAxisRaw("Vertical") * Time.fixedDeltaTime * moveSpeed);
        if (!waiting)
        {
            _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            rb.velocity = _input * moveSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Wall"))
        //{
        //    rb.velocity = Vector3.zero;
        //}
        if (collision.gameObject.CompareTag("Pellet"))
        {
            Wait();

        }
    }
}
