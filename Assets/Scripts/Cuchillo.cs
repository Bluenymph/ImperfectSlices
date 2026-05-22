using System.Collections;
using UnityEngine;

public class Cuchillo : MonoBehaviour
{
    [Header("Movement Configuration")]
    [SerializeField] private float cutLength = 0.3f;
    [SerializeField] private float cutRotation = 60f;
    [SerializeField] private float cutOffset = 0.15f;

    [Header("Time Configuration")]
    [SerializeField] private float downTime = 0.1f;
    [SerializeField] private float upTime = 0.3f;

    private Vector3 _originalPos;
    private Quaternion _originalRot;
    private bool _isCutting = false;

    void Start()
    {
        _originalPos = transform.localPosition;
        _originalRot = transform.localRotation;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !_isCutting)
        {
            StartCoroutine(AnimarCorteCompleto());
        }
    }

    IEnumerator AnimarCorteCompleto()
    {
        _isCutting = true;

        Vector3 finalPos = _originalPos + Vector3.down * cutLength;
        finalPos.x -= cutOffset;
        Quaternion finalRot = _originalRot * Quaternion.Euler(0, 0, cutRotation);

        float time = 0;
        while (time < downTime)
        {
            float t = time / downTime;
            float tSmooth = t * t; 

            transform.localPosition = Vector3.Lerp(_originalPos, finalPos, tSmooth);
            transform.localRotation = Quaternion.Slerp(_originalRot, finalRot, tSmooth);
            
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = finalPos;
        transform.localRotation = finalRot;

        time = 0;
        while (time < upTime)
        {
            float t = time / upTime;
            float tSmooth = t * (2 - t); 

            transform.localPosition = Vector3.Lerp(finalPos, _originalPos, tSmooth);
            transform.localRotation = Quaternion.Slerp(finalRot, _originalRot, tSmooth);
            
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = _originalPos;
        transform.localRotation = _originalRot;

        _isCutting = false;
    }
}
