using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(
            Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed,
            0,
            Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed);
    }
}
