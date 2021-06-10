using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathProjection : MonoBehaviour
{

    public GameObject ballGameObject;
    public BallController ballControllerScript;
    private SphereCollider ballCollider;
    private LineRenderer lineRenderComponent;
    int layerMask = 10;

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = new Vector3(ballControllerScript.transform.position.x, 0, 0);
        transform.position = new Vector3(0, 0, 0);
        lineRenderComponent = gameObject.GetComponent<LineRenderer>();
        lineRenderComponent.SetPosition(0, ballControllerScript.transform.position);
        ballCollider = ballGameObject.GetComponent<SphereCollider>();

        Ray ballRay = new Ray(ballControllerScript.transform.position, ballControllerScript.movementDirection);
        lineRenderComponent.SetPosition(1, ballRay.GetPoint(100f));

    }

    // Update is called once per frame
    void Update()
    {

        Ray ballRay = new Ray(ballControllerScript.transform.position, ballControllerScript.movementDirection);


        if (Physics.Raycast(ballRay.origin, ballRay.direction, out RaycastHit hit))
        {
            Vector3 reflectVector = ballControllerScript.movementDirection;
            //Debug.Log(hit.collider.gameObject.tag == "PaddleHitArea");
            //Debug.Log(hit.collider.name);

            //Debug.DrawLine(ballControllerScript.transform.position, hit.point, Color.green, 1f, false);
            lineRenderComponent.SetPosition(0, ballControllerScript.transform.position);
            lineRenderComponent.SetPosition(1, hit.point);

            if (hit.collider.gameObject.name == "Sidewall")
            {
                reflectVector = new Vector3(-ballControllerScript.movementDirection.x, ballControllerScript.movementDirection.y, ballControllerScript.movementDirection.z);
            }

            if (hit.collider.gameObject.name == "Player")
            {
                reflectVector = new Vector3(ballControllerScript.movementDirection.x, ballControllerScript.movementDirection.y, ballControllerScript.movementDirection.z);
            }

            Ray reflectionRay = new Ray(hit.point, reflectVector);

            if (Physics.Raycast(reflectionRay.origin, reflectionRay.direction, out RaycastHit hitRelfect))
            {
                //Debug.DrawLine(hit.point, hitRelfect.point, Color.green, 1f, false);
                lineRenderComponent.SetPosition(2, hit.point);
                lineRenderComponent.SetPosition(3, hitRelfect.point);
            }
        }
        //Debug.Log(ballRay.GetPoint(100f));
    }
}
