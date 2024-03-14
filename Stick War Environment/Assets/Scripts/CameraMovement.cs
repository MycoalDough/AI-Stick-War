using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float speed;
    public float upDownSpeed = 10;
    public float cam = 10f;
    public Camera cams;
    Transform target;

    public Vector3 minPos, maxPos;

    public float zoomSpeed = 4f;
    float minZoom = 7f;
    float maxZoom = 9.2f;

    // Start is called before the first frame update
    void Start()
    {
        target = cams.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = target.position;

        Vector3 boundPos = new Vector3(
        Mathf.Clamp(target.position.x, minPos.x, maxPos.x),
        Mathf.Clamp(target.position.y, minPos.y, maxPos.y),
        Mathf.Clamp(target.position.z, minPos.z, maxPos.z));

        if (Input.GetKey(KeyCode.D) || (Input.GetKey(KeyCode.RightArrow)))
        {
            if (transform.position.x < maxPos.x)
            {
                transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            }
        }
        if (Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.LeftArrow)))
        {
            if (transform.position.x > minPos.x)
            {
                transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
            }
        }


        cam -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        cam = Mathf.Clamp(cam, minZoom, maxZoom);
        cams.orthographicSize = cam;



    }
}
