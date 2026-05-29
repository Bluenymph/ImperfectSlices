using UnityEngine;
using Random = UnityEngine.Random;

public class Cuchillo : MonoBehaviour
{

    [Header("Properties")] 
    [SerializeField] private Transform cutPoint;
    [SerializeField] private LayerMask cutLayer;
    [SerializeField] private float cutDistance = 2f;
    [SerializeField] private GameObject cutParticles;
    [SerializeField] private AudioSource clangAudioSource;
    public float knifeForce = 1f;

    private Vector3 _originalPos;
    private Quaternion _originalRot;
    private Animator _animator;
    private bool _canCut = true;
    private AudioSource _audioSource;
    
    private static readonly int IsCuttingHash = Animator.StringToHash("isCutting");
    private static readonly int SliceSpeedHas = Animator.StringToHash("sliceSpeed");
    
    private void OnEnable()
    {
        GameManager.OnAddScoreMultiplier += MoreSpeed;
        Spawner.OnLevelEnded += CanNotCut;
    }

    private void OnDisable()
    {
        GameManager.OnAddScoreMultiplier -= MoreSpeed;
        Spawner.OnLevelEnded += CanNotCut;
    }

    void Start()
    {
        _originalPos = transform.localPosition;
        _originalRot = transform.localRotation;
        _animator =  GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && _canCut)
        {
            _animator.SetBool(IsCuttingHash, true);
        }
        else
        {
            _animator.SetBool(IsCuttingHash, false);
        }
    }

    public void EndCutting()
    {
        transform.localPosition = _originalPos;
        transform.localRotation = _originalRot;
    }

    public void ExeSlice()
    {
        _audioSource.Play();
        if (Physics.Raycast(cutPoint.position, -cutPoint.up, out RaycastHit hit, cutDistance, cutLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            
            Vector3 direccionLateral = (Random.value < 0.5f) ? hit.collider.transform.right : -hit.collider.transform.right;

            hit.collider.isTrigger = false;
            rb.isKinematic = false; 
            rb.useGravity = true;
            
            if (hit.collider.CompareTag("HeavyIngredient"))
            { 
                direccionLateral += Vector3.up * 0.5f;
                rb.AddForce(direccionLateral * knifeForce * 2, ForceMode.Impulse);    
            }
            else if(hit.collider.CompareTag("HeavyIngredient") && !hit.collider.CompareTag("CuttingBoard"))
            {
                rb.AddForce(direccionLateral * knifeForce, ForceMode.Impulse);
            }
            if(!hit.collider.CompareTag("CuttingBoard")) 
                rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            
            if (hit.collider.CompareTag("Negative"))
            {
                GameManager.Instance.RemoveScore(10);
            }else if(hit.collider.CompareTag("CuttingBoard"))
            {
                _canCut = false;
                _animator.Play("KnifeStun");
                Invoke(nameof(CanCut), 1f);
                clangAudioSource.Play();
                return;
            }
            else
            {
                GameManager.Instance.AddScore(10);
            }
            hit.collider.tag = "Untagged";
            
            GameObject particles = Instantiate(cutParticles, hit.point, cutPoint.rotation);
            Destroy(particles, 2f);
            GameManager.Instance.hud.UpdateInfoText(GameManager.Instance.score.ToString());
        }
    }

    public void CanNotCut()
    {
        _canCut = false;
    }
    
    public void CanCut()
    {
        _canCut = true;
        _animator.Play("KnifeIdle");
    }

    private void MoreSpeed(int multiplier)
    {
        switch (multiplier)
        {
            case 1:
                _animator.SetFloat(SliceSpeedHas, 1.0f);
                break;
            case 2:
                _animator.SetFloat(SliceSpeedHas, 1.1f);
                knifeForce = 3.1f;
                break;
            case 3:
                _animator.SetFloat(SliceSpeedHas, 1.2f);
                knifeForce = 3.2f;
                break;
            case 4:
                _animator.SetFloat(SliceSpeedHas, 1.3f);
                knifeForce = 3.3f;
                break;
            case 5:
                _animator.SetFloat(SliceSpeedHas, 1.4f);
                knifeForce = 3.4f;
                break;
        }
    }
}
