using UnityEngine;

public class StateInAir : MovementBase
{
    public override void UpdateState(PlayerMovement context)
    {
        Vector3 viewDirection = context.GetViewDirection(context.previousUp);
        Vector3 MoveSum = context.transform.position;
        MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context.currentVelocity * Time.fixedDeltaTime * Vector3.Scale(context.transform.up, new Vector3(-1, -1, -1)), 0.5f) - context.transform.position;
        if (Input.GetAxis("Vertical") != 0)
        {
            MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context.movementSpeed * Time.fixedDeltaTime * viewDirection.normalized * Input.GetAxis("Vertical") + context.movementSpeed * 0.5f * Time.fixedDeltaTime * context.transform.right * Input.GetAxis("Horizontal"), Mathf.Abs(Input.GetAxis("Vertical"))) - context.transform.position;//input on t alows smooth speed up and fade out
            context.rb.MoveRotation(Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(viewDirection.normalized + context.transform.right * Input.GetAxis("Horizontal") * 0.5f, context.previousUp), context.smoothTime));
        }
        else
        {
            MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context.movementSpeed * Time.fixedDeltaTime * context.transform.right * Input.GetAxis("Horizontal"), 0.3f) - context.transform.position;
        }
        context.rb.MovePosition(MoveSum);
        if (context.currentVelocity <= 13)
        {
            context.currentVelocity += Time.fixedDeltaTime * 10;
        }
        if (Physics.Raycast(context.GroundParent.GetChild(0).position, -context.GroundParent.GetChild(0).transform.up, out RaycastHit HitCentre, context.floorDist + 0.05f, context.GroundLayers))
        {
            context.SwitchState(context.stateGround);
            context.currentVelocity = 0f;
        }
    }
}
