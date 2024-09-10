using UnityEngine;

public class ThrowableInjection : IThrowable
{
    private Rigidbody rb;
    private const float Gravity = -9.81f;
    Transform trans;
    float distanceThrow;
    float heightThrow;
    public ThrowableInjection(Rigidbody rigidbody,Transform transform, float distance, float height)
    {
        rb = rigidbody;
        trans = transform;
        distanceThrow = distance;
        heightThrow = height;
    }
    public void Throw()
    {
        rb.isKinematic = false;
        Vector3 targetPosition = rb.transform.position + trans.forward.normalized * distanceThrow;

        Vector3 displacement = targetPosition - rb.transform.position;
        Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);

        float timeToTarget = Mathf.Sqrt(-2 * heightThrow / Gravity) + Mathf.Sqrt(2 * (displacement.y - heightThrow) / Gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * Gravity * heightThrow);
        Vector3 velocityXZ = displacementXZ / timeToTarget;

        rb.velocity = velocityXZ + velocityY;
    }
    public void ApplyGravity()
    {
        rb.velocity += Vector3.up * Gravity * Time.fixedDeltaTime;
    }
}
