using DG.Tweening;
using EZCameraShake;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform leftDoor;
    
    [SerializeField]
    private Transform rightDoor;
    
    [Header("Door Animation")]
    [SerializeField]
    private AnimationCurve doorAnimationCurve;

    [SerializeField] private float animationDuration;

    private Sequence _doorAnimation;
    
    private bool _isOpen = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && ! _isOpen)
        {
            StartDoorAnimation();
        }
    }


    void StartDoorAnimation()
    {
        _doorAnimation?.Complete();
        
        _doorAnimation = DOTween.Sequence();

        var width = leftDoor.GetComponentInChildren<Renderer>().bounds.extents.x;

        CameraShaker.Instance.ShakeOnce(1f, 10, animationDuration, 4f);

        _doorAnimation
            .Append(leftDoor.DOMoveX(leftDoor.position.x - width, animationDuration).SetEase(doorAnimationCurve))
            .Insert(0, rightDoor.DOMoveX(rightDoor.position.x + width, animationDuration).SetEase(doorAnimationCurve))
            .OnComplete(() => _isOpen = true);


        _doorAnimation.Play();
    }
}