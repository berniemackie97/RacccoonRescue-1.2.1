using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapCamera : MonoBehaviour
{
    private Vector2 _prevPosition;
    private Transform _transform;

    public Camera Camera;
    public Bounds Bounds;
    Vector2 firstV;
    Vector2 deltaV;
    private float currentTime;
    private float speed;
    bool touched;

    public void Awake()
    {
        _transform = transform;
        currentTime = 0;
        speed = 0;

    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }

    public void Update()
    {

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
			HandleTouchInput();
#else
        HandleMouseInput();
#endif
    }

    void LateUpdate()
    {

        SetPosition(transform.position);
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touched = true;
                deltaV = Vector2.zero;
                _prevPosition = touch.position;
                firstV = _prevPosition;
                currentTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 curPosition = touch.position;
                MoveCamera(_prevPosition, curPosition);
                deltaV = _prevPosition - curPosition;
                _prevPosition = curPosition;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touched = false;
            }
        }
        else if (!touched)
        {
            deltaV -= deltaV * Time.deltaTime * 10;
            transform.Translate(deltaV.x / 30, deltaV.y / 30, 0);
        }

    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            deltaV = Vector2.zero;
            _prevPosition = Input.mousePosition;
            firstV = _prevPosition;
            currentTime = Time.time;
        }

        else if (Input.GetMouseButton(0))
        {
            Vector2 curMousePosition = Input.mousePosition;
            MoveCamera(_prevPosition, curMousePosition);
            deltaV = _prevPosition - curMousePosition;

            _prevPosition = curMousePosition;
            speed = Time.time;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            speed = (Time.time - currentTime);
            Vector3 diffV = (transform.position - (Vector3)deltaV);
            Vector3 destination = (transform.position - diffV / 20);
        }
        else
        {
            deltaV -= deltaV * Time.deltaTime * 10;
            transform.Translate(deltaV.x / 30, deltaV.y / 30, 0);
        }

    }
    private void MoveCamera(Vector2 prevPosition, Vector2 curPosition)
    {
        if (EventSystem.current.IsPointerOverGameObject(-1))
            return;
        SetPosition(
            transform.localPosition +
            (Camera.ScreenToWorldPoint(prevPosition) - Camera.ScreenToWorldPoint(curPosition)));
    }

    public void SetPosition(Vector2 position)
    {
        Vector2 validatedPosition = ApplyBounds(position);
        _transform = transform;
        _transform.position = new Vector3(validatedPosition.x, validatedPosition.y, _transform.position.z);
    }

    private Vector2 ApplyBounds(Vector2 position)
    {
        float cameraHeight = Camera.orthographicSize * 2f;
        float cameraWidth = (Screen.width * 1f / Screen.height) * cameraHeight;
        position.x = Mathf.Max(position.x, Bounds.min.x + cameraWidth / 2f);
        position.y = Mathf.Max(position.y, Bounds.min.y + cameraHeight / 2f);
        position.x = Mathf.Min(position.x, Bounds.max.x - cameraWidth / 2f);
        position.y = Mathf.Min(position.y, Bounds.max.y - cameraHeight / 2f);
        return position;
    }

}
