using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public enum PlayerStates
{
    Grounded,
    Jump,
    Fall
}

public class PlayerMovementHorizontal : MonoBehaviour
{
    public PlayerData Data;

    private Rigidbody rb;

    private float gravityScale = 1.0f;

    private float lastJumpInputTimer = 0.0f;

    private Vector3 moveDirection;

    private Animator animator;

    private Vector3 initPos;

    private Vector3 initRot;

    private bool move = false;
    private bool gamePaused = false;

    private bool alive = true;

    public PlayerStates state = PlayerStates.Grounded;

    public float rayDistance = 1f;

    public UnityEvent playerLose;
    public UnityEvent stageFinish;

    public string characterName;

    public GameObject dogoPrefab; // Prefab para Dogo
    public GameObject loboPrefab; // Prefab para Lobo
    public GameObject pandaPrefab; // Prefab para Panda

    public GameObject splashPrefab; // Prefab para la animación de salpicadura

    public GameObject slimesplashPrefab; // Prefab para la animación de salpicadura

    public GameObject Estela; // Prefab para la estela

    public GameObject dogoDiePrefab; // Prefab para el hijo que muere

    public GameObject loboDiePrefab; // Prefab para el hijo que muere

    public GameObject pandaDiePrefab; // Prefab para el hijo que muere

    private GameObject prefabToInstantiate; // Prefab a instanciar

    private GameObject DeathPrefabtoInstantiate; // Prefab del nuevo hijo 


    private void generarCollisionsyRigidbody(GameObject Diechild){

        Transform childWithPieces = Diechild.transform.GetChild(0);
        for (int i = 0; i < childWithPieces.childCount; i++)
        {
            Transform piece = childWithPieces.GetChild(i);
            piece.gameObject.AddComponent<Rigidbody>();
            piece.gameObject.AddComponent<BoxCollider>();
        }

    }
    
    private void changeModeltoDie(){
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject); // DESTRUIMOS EL HAZ DE LUZ Y EL MODELO
        }     

        GameObject DieChild = Instantiate(DeathPrefabtoInstantiate, transform);
        DieChild.transform.localPosition = new Vector3(0.167f, 0, 0);; // Ajustar posición relativa al padre
        DieChild.transform.localRotation = Quaternion.Euler(0, -90, 0); // Ajustar rotación relativa al padre
        generarCollisionsyRigidbody(DieChild);

    }

    private void cargarPrefabs(){
        characterName = PlayerPrefs.GetString("playerModel");
        switch (characterName)
        {
            case "DOG":
                prefabToInstantiate = dogoPrefab;
                DeathPrefabtoInstantiate = dogoDiePrefab;
                break;
            case "WOLF":
                prefabToInstantiate = loboPrefab;
                DeathPrefabtoInstantiate = loboDiePrefab;
                break;
            case "PANDA":
                prefabToInstantiate = pandaPrefab;
                DeathPrefabtoInstantiate = pandaDiePrefab;
                break;

            default:
                Debug.LogWarning("No se encontró un prefab para: " + characterName);
                return;
        }

        if (DeathPrefabtoInstantiate == null)
        {
            Debug.LogWarning("No se encontró un prefab para: " + characterName);
        }else{
            Debug.Log("Se encontro el prefab para " + characterName);
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Si el prefab existe, instanciarlo y hacerlo hijo
        if (prefabToInstantiate != null)
        {
            GameObject instantiatedChild = Instantiate(prefabToInstantiate, transform);
            instantiatedChild.transform.localPosition = new Vector3(0.167f, 0, 0); // Ajustar posición relativa al padre
            instantiatedChild.transform.localRotation = Quaternion.Euler(0, -90, 0); // Ajustar rotación relativa al padre
            instantiatedChild.SetActive(true);

            Transform childWithAnimator = instantiatedChild.transform.GetChild(0);

            childWithAnimator.gameObject.SetActive(true);

            animator = childWithAnimator.GetComponent<Animator>();

        }else {
            Debug.LogWarning("No se encontró un prefab para: " + characterName);
        }

        if (Estela != null){
            //Ponemos la estela
            GameObject estela = Instantiate(Estela, transform);
            estela.transform.localPosition = new Vector3(-0.421f, 0, 0); // Ajustar posición relativa al padre
            estela.transform.localRotation = Quaternion.Euler(0, 0, 0); // Ajustar rotación relativa al padre
            estela.SetActive(true);
        }else{
            Debug.LogWarning("No se encontró un prefab para la estela");
        }
    }
    
    void Start()
    {
        cargarPrefabs();

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Desactivamos la gravedad predeterminada
        initPos = transform.position;
        initRot = transform.eulerAngles;

        // Inicia el movimiento hacia la derecha (eje X global positivo)
        moveDirection = Vector3.right;

        // Inicia los listeners
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().playerRun.AddListener(this.SetMove);
        stageFinish.AddListener(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().PlayerEndStage);
        playerLose.AddListener(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().PlayerLose);
        playerLose.AddListener(GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>().ShowLoseMenu);
    }

    void Update()
    {
        // Movimiento horizontal con coordenadas globales
        if (move){
            transform.Translate(moveDirection * Data.playerSpeed * Time.deltaTime, Space.World);
        }

        //Timers
        lastJumpInputTimer -= Time.deltaTime;

        //Input

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !gamePaused) // Si se presiona la barra espaciadora o el botón izquierdo del ratón
        {
            lastJumpInputTimer = Data.jumpInputBufferTime;
        }

        // Detectar entrada para salto
        if (lastJumpInputTimer > 0.0f && state == PlayerStates.Grounded)
        {
            Jump();
        }

        // Cambiar a estado de caída si la velocidad vertical es negativa
        if (state == PlayerStates.Jump && rb.linearVelocity.y < 0)
        {
            state = PlayerStates.Fall;
            SetGravityScale(Data.fallGravityMult); // Aumentar la gravedad en la caída
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alive){ // Si el jugador está vivo

            switch (other.tag)
            {
                case "LeftTurn":
                    // Girar hacia la izquierda: 90 grados negativos
                    RotateGlobal(-90);
                    PositionOnBlock();
                    break;

                case "RightTurn":
                    // Girar hacia la derecha: 90 grados positivos
                    RotateGlobal(90);
                    PositionOnBlock();
                    break;

                case "LevelFinish":
                    //transform.GetChild(0).gameObject.SetActive(false);
                    // Reiniciar posición al inicio
                    transform.position = initPos;
                    transform.eulerAngles = initRot;
                    SetmoveDirection(Vector3.right); // Reinicia dirección hacia la derecha
                    foreach (Transform child in transform)
                    {
                        Destroy(child.gameObject); // DESTRUIMOS EL HAZ DE LUZ Y EL MODELO
                    }
                    stageFinish.Invoke();
                    cargarPrefabs(); // Cargamos el hijo vivo
                    //transform.GetChild(0).gameObject.SetActive(true);
                    break;

                 case "Spikes":
                    // 1. Llama a una Corrutina
                    Debug.Log("RUNNING INTO SPIKES");
                    SpikeSequence();
                    playerLose.Invoke();
                    break;

                case "JumpTrigger":
                    lastJumpInputTimer = Data.jumpInputBufferTime;
                    break;

                case "Sea":
                    // Reiniciar posición al inicio
                    Debug.Log("RUNNING INTO SEA");
                    SplashSequence();
                    playerLose.Invoke();
                    /*transform.position = initPos;
                    transform.eulerAngles = initRot;
                    SetmoveDirection(Vector3.right);*/ // Reinicia dirección hacia la derecha
                    break;

                default:
                    break;
            }
        }
    }

    private void SplashSequence (){
        if (splashPrefab != null)
        {
            alive = false;
            SoundManager.Instance.PlaySFX("splash"); // Reproducir sonido de salpicadura
            GameObject foundObject = GameObject.Find("BGScene");
            GameObject splash = null;
            if (foundObject != null){
                splash = Instantiate(splashPrefab);
            }else{
                foundObject = GameObject.Find("BGScene1");
                if (foundObject != null){
                    splash = Instantiate(slimesplashPrefab);
                }
            }
             // Instanciar el splash sin padre
            splash.transform.position = transform.position; // Colocar en la posición global del jugador
            splash.transform.rotation = Quaternion.Euler(-90, 0, 0); // Ajustar la rotación deseada
            splash.SetActive(true);

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject); // DESTRUIMOS EL HAZ DE LUZ Y EL MODELO
            }    
        }
    }


    private void SpikeSequence()
    {
        alive = false;

        SetMove(false);

        SoundManager.Instance.PlaySFX("impact");
        
        changeModeltoDie();
    }

    private void Jump()
    {
        // Aplicar fuerza de salto
       
        if (animator != null){
            Debug.Log("Saltando...");
            SoundManager.Instance.PlaySFXJump(0.4f);
            animator.SetTrigger("JumpTrigger");
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Reiniciar velocidad vertical
            rb.AddForce(Vector3.up * Data.jumpForce, ForceMode.Impulse);
            state = PlayerStates.Jump;
            Debug.Log("Estado después de saltar: " + state);
        } 
    }

    void OnCollisionEnter(Collision collision)
    {
        state = PlayerStates.Grounded;
        SetGravityScale(1.0f); // Restaurar la gravedad normal
    }

    private void SetGravityScale(float scale)
    {
        gravityScale = scale;
    }

    void FixedUpdate()
    {
        // Aplicar gravedad manualmente
        Vector3 gravity = gravityScale * Data.gravityStrength * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    private void PositionOnBlock()
    {
        // Dirección del rayo (hacia abajo)
        Vector3 direction = Vector3.down;

        // Lanzar el raycast desde la posición del objeto
        if (Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, rayDistance) && !(hitInfo.collider.CompareTag("LeftTurn") && hitInfo.collider.CompareTag("RightTurn")))
        {
            // Si el raycast golpea un objeto, obtiene su Collider
            Collider blockCollider = hitInfo.collider;

            // Calcula el centro del bloque usando el bounding box del Collider
            Vector3 blockCenter = blockCollider.bounds.center;

            // Posiciona al personaje en el centro del bloque (ajusta la altura si es necesario)
            Vector3 newPosition = new Vector3(blockCenter.x, transform.position.y, blockCenter.z);
            transform.position = newPosition;

            Debug.Log($"Posicionado sobre el bloque: {blockCollider.gameObject.name}");
            Debug.Log($"Centro del bloque: {blockCenter}");
        }
        else
        {
            Debug.Log("No hay bloques debajo.");
        }
    }

    private void RotateGlobal(float angle)
    {
        // Rotar el objeto en el eje Y global
        transform.Rotate(Vector3.up, angle, Space.World);

        // Actualizar la dirección global basada en la rotación
        moveDirection = Quaternion.Euler(0, angle, 0) * moveDirection;
    }

    Vector3 FixCoords(Vector3 coords)
    {
        // Redondear las coordenadas globales
        return new Vector3(Mathf.RoundToInt(coords.x), coords.y, Mathf.RoundToInt(coords.z));
    }

        // Método para obtener el valor del atributo privado
    public Vector3 GetmoveDirection()
    {
        return moveDirection;
    }

    // Método para establecer el valor del atributo privado
    public void SetmoveDirection(Vector3 value)
    {
        moveDirection = value;
    }

    public void SetMove(bool b)
    { 
        if (b)
            SoundManager.Instance.PlayLoopSound("grass", 0.2f);
        else
            SoundManager.Instance.StopLoopSound();
        move = b;
    }

    public void SetGamePaused(bool b)
    {
        gamePaused = b;
    }
}
