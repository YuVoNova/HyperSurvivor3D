using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rigidbody;

    [SerializeField]
    private GameObject Joystick;
    [SerializeField]
    private RectTransform Handle;

    [SerializeField]
    private float clickTreshold;
    [SerializeField]
    private float dragMultiply;
    [SerializeField]
    private float rotationSpeed;

    private Touch touch;

    private bool clickFlag;
    private bool hasGameStarted;
    private bool isJoystickActive;

    private Vector2 delta;
    private Vector2 clickCenter;
    private Vector2 direction2D;
    private Vector3 direction;
    private Vector3 targetDirection;

    private Quaternion targetRotation;

    private float MovementSpeed;

    private Vector2 handleRadius;
    private Vector2 handleDirection;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        Joystick.SetActive(false);

        direction = Vector3.zero;
        targetDirection = Vector3.zero;

        handleRadius = Joystick.GetComponent<RectTransform>().sizeDelta / 2f;
        handleDirection = Vector2.zero;

        clickFlag = false;
        hasGameStarted = false;
        isJoystickActive = false;
    }

    private void Update()
    {
        if (hasGameStarted)
        {
            if (GameManager.Instance.IsGameOn)
            {
                if (Input.touches.Length > 0)
                {
                    touch = Input.touches[0];
                    delta = touch.deltaPosition;

                    if (touch.phase == TouchPhase.Began)
                    {
                        Joystick.SetActive(true);

                        clickFlag = true;
                        isJoystickActive = false;
                    }
                    else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        if (clickFlag)
                        {
                            clickCenter = touch.position;

                            clickFlag = false;
                            isJoystickActive = true;

                            GetComponent<Player>().BaseClass.Animator.SetBool("isWalk", true);
                        }

                        if (delta.magnitude > clickTreshold)
                        {
                            direction2D = touch.position - clickCenter;
                            direction = new Vector3(direction2D.x, 0.0f, direction2D.y);
                            direction = Vector3.Normalize(direction);

                            handleDirection = direction2D / 100f;
                            if (handleDirection.magnitude > 1f)
                            {
                                handleDirection = handleDirection.normalized;
                            }
                        }
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        if (delta.magnitude < clickTreshold && clickFlag)
                        {
                            Click(touch);
                        }

                        Handle.anchoredPosition = Vector2.zero;
                        Joystick.SetActive(false);

                        clickFlag = false;
                        isJoystickActive = false;

                        GetComponent<Player>().BaseClass.Animator.SetBool("isWalk", false);
                    }
                }
                else
                {
                    GetComponent<Player>().BaseClass.Animator.SetBool("isWalk", false);

                    direction = transform.forward;
                }
            }
        }
        else
        {
            if (Input.touches.Length > 0)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    GameManager.Instance.GameUIManager.PrepareClassSelection();

                    hasGameStarted = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOn)
        {
            if (GetComponent<Player>().BaseClass.ClosestTargetTransform != null)
            {
                targetDirection = Vector3.Normalize(GetComponent<Player>().BaseClass.ClosestTargetTransform.position - transform.position);
                targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            if (isJoystickActive)
            {
                Handle.anchoredPosition = handleDirection * handleRadius;

                direction.y = 0f;
                rigidbody.velocity = direction * MovementSpeed * Time.fixedDeltaTime;
            }
        }
    }

    private void Click(Touch touch)
    {

    }

    public void UpgradeMovementSpeed(float value)
    {
        MovementSpeed = value;
    }
}
