using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour {
    public LayerMask whatIsGrappleable;
    public Transform grappleSource, targetSource, player;
    public float maxDistance = 100f;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    private SpringJoint joint;

    void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            startGrapple();
        } else if (Input.GetMouseButtonUp(0)) {
            stopGrapple();
        }
    }

    void LateUpdate() {
        DrawRope();
    }

    void startGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(targetSource.position, targetSource.forward, out hit, maxDistance, whatIsGrappleable)) {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.2f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
        }
    }

    void stopGrapple() {
        lr.positionCount = 0;
        Destroy(joint);
    }

    void DrawRope() {
        if (!joint) { return; }

        lr.SetPosition(0, grappleSource.position);
        lr.SetPosition(1, grapplePoint);
    }
}
