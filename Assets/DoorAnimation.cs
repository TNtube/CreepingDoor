using System;
using System.Collections.Generic;
using DG.Tweening;
using EZCameraShake;
using UnityEngine;
using Random = UnityEngine.Random;


struct LeafData
{
    public Transform Leaf;
    public Vector3 BaseRotation;
}

public class DoorAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform lantern;
    [SerializeField]
    private Light lanternLight;
    [SerializeField]
    private Color baseLanternColor;
    [SerializeField]
    private Color goalLanternColor;
    [SerializeField]
    private Transform backLeftDoor;
    [SerializeField]
    private Transform backRightDoor;
    [SerializeField]
    private Transform frontLeftDoor;
    [SerializeField]
    private Transform frontRightDoor;
    
    [SerializeField]
    private List<Transform> leafs = new List<Transform>();

    [SerializeField]
    private List<Transform> barsList = new List<Transform>();

    [SerializeField]
    private Transform lockEye;

    [Header("Door Animation")]
    [SerializeField]
    private float leafDistanceMax = 6f;
    [SerializeField]
    private float leafDistanceMin = 3.5f;
    
    [SerializeField]
    private AnimationCurve barsAnimationCurve;
    [SerializeField] private float barsDuration = 10;
    
    [SerializeField]
    private AnimationCurve backDoorAnimationCurve;
    [SerializeField] private float backDoorDuration = 10;
    
    [SerializeField]
    private AnimationCurve frontDoorAnimationCurve;
    [SerializeField] private float frontDoorDuration = 10;

    private List<LeafData> _leafsData = new ();
    private Sequence _doorAnimation;
    private bool _isOpen = false;

    [SerializeField] private ParticleSystem dust;
    
    float leafVibrationElapsedTime = 0f;
    
    
    float _timeInside = 0f;

    [SerializeField]
    private float maxTimeInside = 3f;

    private void Awake()
    {
        foreach (var leaf in leafs)
        {
            _leafsData.Add(new LeafData{Leaf = leaf, BaseRotation = leaf.localEulerAngles});
        }

        dust.Pause();
    }
    
    static float Remap(float v, float a, float b, float c, float d)
    {
        return c + (d - c) * ((v - a) / (b - a));
    }

    // Update is called once per frame
    void Update()
    {
        if (_isOpen) return;
        var dist = Vector3.Distance(lockEye.position, lantern.position);
        var t = Mathf.InverseLerp(leafDistanceMin, leafDistanceMax, dist);
        // print(t);

        lanternLight.color = Color.Lerp(baseLanternColor, goalLanternColor, 1f - t);
        
        var maxInterval = 0.1f;
        var minInterval = 0.001f;
        
        var interval = Remap(t, 0, 1, minInterval, maxInterval);


        if (leafVibrationElapsedTime > interval)
        {
            foreach (var leafData in _leafsData)
            {
                var leaf = leafData.Leaf;
                var baseRotation = leafData.BaseRotation;

                var ry = Random.Range(-5, 5) * (1f - t);
                baseRotation.y += ry;
            
                // vibrate the leaf from the base rotation each frames
                leaf.localEulerAngles = baseRotation;
            }
            leafVibrationElapsedTime = 0f;
        }
        
        leafVibrationElapsedTime += Time.deltaTime;

        _timeInside += t <= 0.01 ? Time.deltaTime : 0;


        if (_timeInside >= maxTimeInside && !_isOpen)
        {
            StartDoorAnimation();
        }
    }


    void StartDoorAnimation()
    {
        _isOpen = true;

        foreach (var leafData in _leafsData)
        {
            var leaf = leafData.Leaf;
            var baseRotation = leafData.BaseRotation;
            print(baseRotation);

            var ry = baseRotation.y >= 270 ? 180 : 0;

            leaf.localEulerAngles = new Vector3(baseRotation.x, ry, baseRotation.z);
        }
        lockEye.GetChild(0).gameObject.SetActive(true);
        
        _doorAnimation?.Complete();
        
        _doorAnimation = DOTween.Sequence();


        float barInterval = 1f;
        int index = 0;

        _doorAnimation.AppendInterval(barsList.Count * barInterval);
        dust.Play();
        CameraShaker.Instance.ShakeOnce(1, 10,
            barsList.Count * barInterval + 6 + backDoorDuration * 2 + frontDoorDuration * 2, 3f);

        foreach (var bar in barsList)
        {
            _doorAnimation.Insert(index * barInterval, bar
                .DOBlendableLocalMoveBy((index % 2 == 0 ? Vector3.left : Vector3.right) * 3f, barsDuration)
                .SetEase(barsAnimationCurve)
            );
            index++;
        }

        _doorAnimation.Append(backLeftDoor.DOLocalMoveZ(-0.6f, 3f).SetEase(Ease.OutExpo));
        _doorAnimation.Join(backRightDoor.DOLocalMoveZ(-0.6f, 3f).SetEase(Ease.OutExpo));
        _doorAnimation.Append(backLeftDoor.DOLocalMoveX(2f, backDoorDuration).SetEase(backDoorAnimationCurve));
        _doorAnimation.Join(backRightDoor.DOLocalMoveX(-2f, backDoorDuration).SetEase(backDoorAnimationCurve));
        _doorAnimation.Join(frontLeftDoor.DOLocalMoveY(6.5f, frontDoorDuration).SetEase(frontDoorAnimationCurve));
        _doorAnimation.Join(frontRightDoor.DOLocalMoveY(-6.5f, frontDoorDuration).SetEase(frontDoorAnimationCurve));
        _doorAnimation.Append(lockEye.GetComponentInChildren<Light>().DOColor(Color.black, 2f));

        _doorAnimation.OnComplete(() =>
        {
            dust.Pause();
            dust.gameObject.SetActive(false);
            lockEye.GetChild(0).gameObject.SetActive(false);
        });


        _doorAnimation.Play();
    }
}