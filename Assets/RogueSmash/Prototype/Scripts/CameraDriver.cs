using UnityEngine;

public class CameraDriver : MonoBehaviour
{
	/// <summary>
	/// Target is what the camera looks at.
	/// </summary>
	[SerializeField] private Transform target;
	[SerializeField] private float verticalOffset;
	[SerializeField] private float followOffset;
	[SerializeField] private float followSpeed = 2.0f;
	[SerializeField] private bool shouldLookAtTarget;
	
	private Vector3 targetPosition;
	private Vector3 targetDirection;

    private void FixedUpdate()
    {
        Move(); // Mueve la cámara suavemente
    }

    private void LateUpdate()
    {
        Look(); // Ajusta la rotación después del movimiento
    }

    private void Move()
	{
		targetPosition = new Vector3(target.transform.position.x, target.transform.position.y + verticalOffset, target.transform.position.z - followOffset);
		targetDirection = targetPosition - transform.position;
		transform.position += targetDirection * Time.deltaTime * followSpeed;
	}

	private void Look()
	{
		/*if (shouldLookAtTarget)
		{
			transform.LookAt(target);
		}*/

		if (shouldLookAtTarget)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followSpeed);
        }
    }
}
