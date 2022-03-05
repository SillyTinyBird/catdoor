using UnityEngine;

public class StateOnEdge : MovementBase
{
    public override void UpdateState(PlayerMovement context)
    {
        Vector3 floor;
        bool notHits = context.FloorAngleCheck(out floor, out float avgDistance);//geting flor mormal vector info
        Vector3 viewDirection = context.GetViewDirection(context.previousUp);
        Vector3 MoveSum = context.transform.position;
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
        if (floor == Vector3.zero)
        {
            context.SwitchState(context.stateAir);
        }
        if (notHits)
        {
            context.SwitchState(context.stateGround);
        }
    }
}
