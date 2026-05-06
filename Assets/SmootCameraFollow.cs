using UnityEngine;

public class SmootCameraFollow : MonoBehaviour
{
    public Transform target;
    public float speed, scaleSpeed;
    public Camera cam;
    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        Vector3 positionLerp = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
        positionLerp.z = transform.position.z;

        transform.position = positionLerp;

        if (cam != null)
        {
            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize,
                5 * target.localScale.x,
                Time.deltaTime * scaleSpeed
            );
        }
    }
}
