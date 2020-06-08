using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private LineRenderer _lineRenderer;
    private Transform _closest;
    private SpringJoint2D _springJoint2D;

    private float _maxSpeed = 50f;

    public GameObject hitEffect;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // get the closest hinge
            _closest = GetClosestHinge(GameObject.FindGameObjectsWithTag("Hinge"));
            // attach joint to it
            _springJoint2D = _closest.GetComponent<SpringJoint2D>();
            _springJoint2D.connectedBody = _rigidbody2D;
            // set line renderer position count
            _lineRenderer.positionCount = 2;
        }

        if (Input.GetMouseButton(0))
        {
            // draw line from player to hinge
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _closest.position);

            if (_springJoint2D.distance > 3)
            {
                _springJoint2D.distance -= 0.01f;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // remove line and hinge
            _lineRenderer.positionCount = 0;
            _springJoint2D.connectedBody = null;
        }
    }

    private void FixedUpdate()
    {
        if (_rigidbody2D.velocity.magnitude > _maxSpeed)
        {
            _rigidbody2D.velocity = _rigidbody2D.velocity.normalized * _maxSpeed;
        }
    }

    Transform GetClosestHinge(GameObject[] hinges)
    {
        Transform closest = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject potentialTarget in hinges)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (!(dSqrToTarget < closestDistanceSqr))
            {
                continue;
            }
            closestDistanceSqr = dSqrToTarget;
            closest = potentialTarget.transform;
        }

        return closest;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Instantiate(hitEffect, collision.transform.position, Quaternion.identity);
        Destroy(collision.gameObject);
    }
}