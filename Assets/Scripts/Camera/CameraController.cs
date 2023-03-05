using UnityEngine;

/// <summary>
/// Скрипт, предназначенный для управления камерой.
/// </summary>
public class CameraController : MonoBehaviour
{
    public GameManager gameManager;

    [SerializeField] private float speed = 6f;
    [SerializeField] private float zoomSpeed;
    private Vector3 defaultPosition = new Vector3(-0.38f, 6f, -4.05f);
    private Vector3 lastPosition;

    // Объект, на котором камера сфокусируется.
    [SerializeField] private GameObject objectToFocusOn;
    // Расстояние камеры от объекта фокуса.
    [SerializeField] private Vector3 distanceFromObject;

    private void Start()
    {
        transform.position = defaultPosition;
        lastPosition = transform.position;

        isLocked = false;
        cameraState = 0;
    }

    [SerializeField] private float horizontalBoarderOnTheRight;
    [SerializeField] private float horizontalBoarderOnTheLeft;

    [SerializeField] private float verticalBoarderOnTheForward;
    [SerializeField] private float verticalBoarderOnTheBackward;

    [SerializeField] private float verticalBoarderOnTheUp;
    [SerializeField] private float verticalBoarderOnTheDown;

    /// 0 - Свободная, 1 - Зафиксированная на определённом месте. Новые состояния могут быть добавлены.
    [SerializeField] private int cameraState;
    [SerializeField] private bool isLocked;
    private void Update()
    {
        if (!isLocked)
        {
            switch (cameraState)
            {
                // Свободная камера. Ей может управлять игрок.
                case 0:
                    Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f,
                        Input.GetAxisRaw("Vertical")).normalized * Time.deltaTime * speed;
                    transform.position = new Vector3(transform.position.x + direction.x, transform.position.y + direction.y, transform.position.z + direction.z);

                    direction = new Vector3(0f, -Input.GetAxisRaw("Mouse ScrollWheel"), 0f).normalized * Time.deltaTime * zoomSpeed;
                    transform.position = new Vector3(transform.position.x, transform.position.y + direction.y, transform.position.z);

                    // Обработаем ограничения движения.
                    // По горизонтали.
                    if (transform.position.x > horizontalBoarderOnTheRight)
                        transform.position = new Vector3(horizontalBoarderOnTheRight, transform.position.y, transform.position.z);
                    if (transform.position.x < horizontalBoarderOnTheLeft)
                        transform.position = new Vector3(horizontalBoarderOnTheLeft, transform.position.y, transform.position.z);
                    // Приблежение.
                    if (transform.position.y > verticalBoarderOnTheUp)
                        transform.position = new Vector3(transform.position.x, verticalBoarderOnTheUp, transform.position.z);
                    if (transform.position.y < verticalBoarderOnTheDown)
                        transform.position = new Vector3(transform.position.x, verticalBoarderOnTheDown, transform.position.z);
                    // По вертикали.
                    if (transform.position.z > verticalBoarderOnTheForward)
                        transform.position = new Vector3(transform.position.x, transform.position.y, verticalBoarderOnTheForward);
                    if (transform.position.z < verticalBoarderOnTheBackward)
                        transform.position = new Vector3(transform.position.x, transform.position.y, verticalBoarderOnTheBackward);

                    lastPosition = transform.position;
                    break;
                // Зафиксированная на определённом месте.
                case 1:
                    // При нажатии на ESC или ПКМ, освобождаем камеру и закрываем все интерфейсные элементы.
                    if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                    {
                        FreeCamera();
                        gameManager.districtUI.Close();
                    }
                    break;
                // Камера отключена.
                default:
                    break;
            }
        }
        
        if (objectToFocusOn != null)
        {
            transform.position = objectToFocusOn.transform.position + distanceFromObject;
        }
    }

    /// <summary>
    /// Переводит камеру в режим "Свободная". И возвращает её на место до фокусировки.
    /// </summary>
    public void FreeCamera()
    {
        //Debug.Log("Переводит камеру в режим \"Свободная\"");
        objectToFocusOn = null;
        transform.position = lastPosition;
        //isLocked = false;
        cameraState = 0;
    }

    /// <summary>
    /// Переводит камеру в режим "Зафиксированная на определённом месте", если можно.
    /// </summary>
    /// <param name="focusObject">Объект, на месте которого нужно зафиксироать камеру.</param>
    public void FocusCamera(GameObject focusObject)
    {
        if (gameManager.developerState)
        {
            //gameManager.nearbyDistrictsCreator.DistrictAction(focusObject.GetComponent<District>());
        }
        else
        {
            if (cameraState == 0)
            {
                if (focusObject.CompareTag("District"))
                {
                    // Отобразим панель Района и сформируем её, беря данные из района-информации.
                    gameManager.districtUI.Open(focusObject.GetComponent<District>().districtInfo);
                }
                objectToFocusOn = focusObject;
                cameraState = 1;
            }
        }
    }

    /// <summary>
    /// Переводит камеру в режим "Отключённая".
    /// </summary>
    public void LockCamera(bool toLock)
    {
        if (toLock)
        {
            isLocked = true;
        }
        else
        {
            isLocked = false;
        }
    }
}
