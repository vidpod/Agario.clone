using UnityEngine;

public class SmootCameraFollow : MonoBehaviour
{
    public Transform target;
    public Camera cam;

    [Header("Follow")]
    public float followSpeed = 10f;

    [Header("Zoom")]
    public float zoomSpeed = 8f;          // faster response
    public float baseOrthoSize = 6f;
    public float zoomFactor = 8f;       // much stronger zoom out
    [Range(0.3f, 1f)] public float zoomPower = 0.9f; // more linear, stronger at high scale
    public float minOrthoSize = 6f;
    public float maxOrthoSize = 70f;      // allow farther zoom out

    void Update()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position;
        desiredPos.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSpeed);

        if (cam != null && cam.orthographic)
        {
            // Prefer total mass (including split cells) if Movement component is available,
            // otherwise fall back to the target's localScale.
            float scaleMetric = 1f;
            Movement movement = target.GetComponent<Movement>();
            if (movement != null)
            {
                scaleMetric = Mathf.Max(1f, movement.GetTotalMass());
            }
            else
            {
                scaleMetric = Mathf.Max(1f, target.localScale.x);
            }

            // Growth based on mass/scale (keeps previous behavior but uses total mass)
            float scaledGrowth = Mathf.Pow(scaleMetric - 1f, zoomPower);

            // Also optionally add extra zoom based on how far split cells are spread out.
            float spreadSize = 0f;
            if (movement != null)
            {
                var cells = movement.GetAllCells();
                Vector2 center = target.position;
                float maxDist = 0f;
                foreach (var cell in cells)
                {
                    if (cell == null) continue;
                    float d = Vector2.Distance(center, cell.transform.position);
                    if (d > maxDist) maxDist = d;
                }
                // tune multiplier to convert world distance to ortho size contribution
                spreadSize = maxDist * 1.5f;
            }

            float targetSize = baseOrthoSize + (scaledGrowth * zoomFactor) + spreadSize;
            targetSize = Mathf.Clamp(targetSize, minOrthoSize, maxOrthoSize);

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);
        }
    }
}