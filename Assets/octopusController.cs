using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class octopusController : MonoBehaviour
{
    public float rotationSpeed;
    public float angularDrag;
    public float moveSpeed;

    public LayerMask grappleableSurfaces, enemySurfaces;
    public Transform targetSource;
    public float maxDistance = 200f;

    public float maxGrappleLength;
    public float minGrappleLength;
    public float spring;
    public float damper;
    public float massScale;

    private Rigidbody rigidBody;
    private LineRenderer lineRenderer;
    private Vector3 grapplePoint;
    private SpringJoint joint;

    void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();

        rigidBody.angularDrag = angularDrag;
    }

    // Update is called once per frame
    void Update() {
        rotate();
        move();
        grapple();
    }

    void LateUpdate() {
        DrawRope();
    }

    private void rotate() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float rotationalForce = horizontalInput * rotationSpeed * Time.fixedDeltaTime;

        Vector3 torque = new Vector3(0, rotationalForce, 0);

        rigidBody.AddTorque(torque, ForceMode.Force);
    }

    private void move() {
        float verticalInput = Input.GetAxis("Vertical");
        float verticalForce = verticalInput * moveSpeed * Time.fixedDeltaTime;

        Vector3 force = transform.forward * verticalForce;

        rigidBody.AddForce(force, ForceMode.Force);
    }

    private void grapple() {
        if (Input.GetButtonDown("Jump")) {
            startGrapple();
        } else if (Input.GetButtonUp("Jump")) {
            stopGrapple();
        }
    }

    private void startGrapple() {
        RaycastHit target = fetchRaycastTarget();

        if (!target.collider) { return; }

        grapplePoint = target.point;
        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

        joint.maxDistance = distanceFromPoint * maxGrappleLength;
        joint.minDistance = distanceFromPoint * minGrappleLength;

        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;

        lineRenderer.positionCount = 2;
    }

    private void stopGrapple() {
        lineRenderer.positionCount = 0;
        Destroy(joint);
    }

    private void DrawRope() {
        if (!joint) { return; }

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    private RaycastHit fetchRaycastTarget() {
        RaycastHit hit;

        bool hitEnemy = Physics.Raycast(
            transform.position,
            transform.forward,
            out hit,
            maxDistance,
            enemySurfaces
        );

        if (hit.collider) { return hit; }
        
        bool hitWall = Physics.Raycast(
            transform.position,
            transform.forward,
            out hit,
            maxDistance,
            grappleableSurfaces
        );

        return hit;
    }
}
