using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public Camera cam;
    public float speed = 2f;

    void Update()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();

        Vector3 mousePos = new Vector3(mouseScreen.x, mouseScreen.y, 5f);
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);

        transform.position = Vector3.MoveTowards(
            transform.position,
            worldPos,
            speed * Time.deltaTime
        );
    }
}