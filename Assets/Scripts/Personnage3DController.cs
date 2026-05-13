using UnityEngine;

/// <summary>
/// Contrôleur de personnage 3D simple pour Unity 6.
/// Ajoutez ce script sur un GameObject possédant un CharacterController.
/// Les touches ZQSD pilotent le déplacement, Espace déclenche le saut, et la caméra suit derrière.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Personnage3DController : MonoBehaviour
{
    [Header("Déplacement")]
    [SerializeField] private float vitesseMarche = 5f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float tempsRotation = 0.08f;

    [Header("Saut et gravité")]
    [SerializeField] private float hauteurSaut = 1.6f;
    [SerializeField] private float gravite = -24f;
    [SerializeField] private float vitessePlaquageSol = -2f;

    [Header("Caméra")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool verrouillerCurseurAuDemarrage = true;
    [SerializeField] private Vector2 sensibiliteSouris = new Vector2(140f, 90f);
    [SerializeField] private float distanceCamera = 5f;
    [SerializeField] private float hauteurCamera = 2f;
    [SerializeField] private float angleMinCamera = -25f;
    [SerializeField] private float angleMaxCamera = 65f;
    [SerializeField] private float tempsLissageCamera = 0.08f;

    private CharacterController characterController;
    private Vector3 vitesseHorizontale;
    private Vector3 vitesseHorizontaleLissage;
    private float vitesseVerticale;
    private float vitesseRotationLissage;
    private float yawCamera;
    private float pitchCamera = 15f;
    private Vector3 vitesseCameraLissage;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        yawCamera = transform.eulerAngles.y;
    }

    private void Start()
    {
        if (verrouillerCurseurAuDemarrage)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        Vector2 entreeDeplacement = LireEntreeDeplacement();
        MettreAJourRotationCamera();
        DeplacerPersonnage(entreeDeplacement);
        GererCurseur();
    }

    private void LateUpdate()
    {
        SuivreAvecCamera();
    }

    private Vector2 LireEntreeDeplacement()
    {
        Vector2 entree = Vector2.zero;

        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow))
        {
            entree.y += 1f;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            entree.y -= 1f;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            entree.x += 1f;
        }

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            entree.x -= 1f;
        }

        return Vector2.ClampMagnitude(entree, 1f);
    }

    private void MettreAJourRotationCamera()
    {
        yawCamera += Input.GetAxisRaw("Mouse X") * sensibiliteSouris.x * Time.deltaTime;
        pitchCamera -= Input.GetAxisRaw("Mouse Y") * sensibiliteSouris.y * Time.deltaTime;
        pitchCamera = Mathf.Clamp(pitchCamera, angleMinCamera, angleMaxCamera);
    }

    private void DeplacerPersonnage(Vector2 entreeDeplacement)
    {
        Vector3 avantCamera = Vector3.ProjectOnPlane(Quaternion.Euler(0f, yawCamera, 0f) * Vector3.forward, Vector3.up).normalized;
        Vector3 droiteCamera = Quaternion.Euler(0f, yawCamera, 0f) * Vector3.right;
        Vector3 directionVoulue = (avantCamera * entreeDeplacement.y + droiteCamera * entreeDeplacement.x).normalized;
        Vector3 vitesseVoulue = directionVoulue * vitesseMarche;

        vitesseHorizontale = Vector3.SmoothDamp(
            vitesseHorizontale,
            vitesseVoulue,
            ref vitesseHorizontaleLissage,
            acceleration > 0f ? 1f / acceleration : 0f);

        if (directionVoulue.sqrMagnitude > 0.001f)
        {
            float angleVise = Mathf.Atan2(directionVoulue.x, directionVoulue.z) * Mathf.Rad2Deg;
            float angleLisse = Mathf.SmoothDampAngle(transform.eulerAngles.y, angleVise, ref vitesseRotationLissage, tempsRotation);
            transform.rotation = Quaternion.Euler(0f, angleLisse, 0f);
        }

        if (characterController.isGrounded && vitesseVerticale < 0f)
        {
            vitesseVerticale = vitessePlaquageSol;
        }

        if (characterController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            vitesseVerticale = Mathf.Sqrt(hauteurSaut * -2f * gravite);
        }

        vitesseVerticale += gravite * Time.deltaTime;

        Vector3 mouvement = vitesseHorizontale + Vector3.up * vitesseVerticale;
        characterController.Move(mouvement * Time.deltaTime);
    }

    private void SuivreAvecCamera()
    {
        if (cameraTransform == null)
        {
            return;
        }

        Quaternion rotationCamera = Quaternion.Euler(pitchCamera, yawCamera, 0f);
        Vector3 pivot = transform.position + Vector3.up * hauteurCamera;
        Vector3 positionVoulue = pivot - rotationCamera * Vector3.forward * distanceCamera;

        cameraTransform.position = Vector3.SmoothDamp(
            cameraTransform.position,
            positionVoulue,
            ref vitesseCameraLissage,
            tempsLissageCamera);
        cameraTransform.rotation = rotationCamera;
    }

    private void GererCurseur()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnValidate()
    {
        vitesseMarche = Mathf.Max(0f, vitesseMarche);
        acceleration = Mathf.Max(0.01f, acceleration);
        tempsRotation = Mathf.Max(0f, tempsRotation);
        hauteurSaut = Mathf.Max(0f, hauteurSaut);
        gravite = Mathf.Min(-0.01f, gravite);
        distanceCamera = Mathf.Max(0.1f, distanceCamera);
        hauteurCamera = Mathf.Max(0f, hauteurCamera);
        tempsLissageCamera = Mathf.Max(0f, tempsLissageCamera);
        angleMinCamera = Mathf.Min(angleMinCamera, angleMaxCamera);
        angleMaxCamera = Mathf.Max(angleMaxCamera, angleMinCamera);
    }
}
