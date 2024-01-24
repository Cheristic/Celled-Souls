using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Main { get; private set; }
    [SerializeField] float mouseSensitivity = 0.01f;
    [SerializeField] float zoomSpeed = 10f;
    private Vector3 prevLocation;
    public float max_x;
    public float min_x;
    public float max_y;
    public float min_y;
    public float max_zoom;
    public float min_zoom;

    private void Start()
    {
        Main = this;
    }

    // https://discussions.unity.com/t/pan-camera-with-mouse/92410/2
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            prevLocation = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            Vector3 change = (Input.mousePosition - prevLocation)*(-mouseSensitivity);
            if ((transform.position.x < min_x && change.x < 0) ||
                (transform.position.x > max_x && change.x > 0))
            {
                change.x = 0;
            }
            if ((transform.position.y < min_y && change.y < 0) ||
                (transform.position.y > max_y && change.y > 0))
            {
                change.y = 0;
            }
            transform.Translate(new Vector3(change.x, change.y, 0));
            prevLocation = Input.mousePosition;
        }

        // OLD ZOOM FEATURE
        /*orthoSize = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if ((Camera.main.orthographicSize <= min_zoom && orthoSize > 0) ||
            (Camera.main.orthographicSize >= max_zoom && orthoSize < 0)) return;
        Camera.main.orthographicSize -= orthoSize;*/
    }
}
