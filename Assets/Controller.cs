using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]

public class Controller : MonoBehaviour

{
    public float speed = 6.0f;
    private GameObject cameraFPS;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private float rotacaoX = 0.0f;
    private float rotacaoY = 0.0f;
    public Rigidbody projectile;
    public float speedshooter = 100;
    public AudioSource tiro;
    public static int vidas;
    public Text txtVida;
    public RectTransform imgVida;

    void Start()
    {
        vidas = 100;
        //som do tiro
        tiro = GetComponents<AudioSource>()[0];

        cameraFPS = GetComponentInChildren(typeof(Camera)).transform.gameObject;
        cameraFPS.transform.localPosition = new Vector3(0,1,0);
        cameraFPS.transform.localRotation = Quaternion.identity;
        controller = GetComponent<CharacterController>();

        // esconder mouse
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        txtVida.text = "Vidas: " + vidas;
        imgVida.sizeDelta = new Vector2 ( 150 * vidas / 100, 22);

        //apenas movimenta o jogador se ele estiver no chão
        if (controller.isGrounded) {
            //pega a direção da face à frente da camera
            Vector3 direcaoFrente = new Vector3(cameraFPS.transform.forward.x, 0, cameraFPS.transform.forward.z);
            //pega a direção da face ao lado da camera
            Vector3 direcaoLado = new Vector3(cameraFPS.transform.right.x, 0, cameraFPS.transform.right.z);
            //normaliza os valores para o máximo de 1, para que o jogador ande sempre na mesma velocidade
            direcaoFrente.Normalize();
            direcaoLado.Normalize();

            //pega o valor das teclas para cima (1) e para baixo  (-1)
            direcaoFrente = direcaoFrente * Input.GetAxis("Vertical");
            //pega o valor das teclas para direita (1) e para esquerda  (-1)
            direcaoLado = direcaoLado * Input.GetAxis("Horizontal");

            //soma a movimentação lateral com a movimentação para frente/trás
            Vector3 direcaoFinal = direcaoFrente + direcaoLado;
            if (direcaoFinal.sqrMagnitude>1) {
                direcaoFinal.Normalize();
            }
            
            //apenas move nas direções x e z
            moveDirection = new Vector3(direcaoFinal.x, 0, direcaoFinal.z);

            //multiplica pela velocidade que foi configurada no jogador
            moveDirection = moveDirection * speed;
            
            //pular
            if (Input.GetButton(("Jump"))) {
                moveDirection.y = 8.0f;
            }

        if (Input.GetButtonDown("Fire1")) {
                Rigidbody hitPlayer;
                hitPlayer = Instantiate(projectile, cameraFPS.transform.position, cameraFPS.transform.rotation) as Rigidbody;
                hitPlayer.velocity = cameraFPS.transform.TransformDirection(Vector3.forward * speedshooter);
                tiro.Play();
            }
        }

        //faz o jogador ir pra baixo (gravidade)
        moveDirection.y -= 20.0f * Time.deltaTime;

        //faz o movimento
        controller.Move(moveDirection * Time.deltaTime);

        cameraPrimeiraPessoa();
    }


    // controle do mouse
    void cameraPrimeiraPessoa(){
        rotacaoX += Input.GetAxis("Mouse X") * 10.0f;
        rotacaoY += Input.GetAxis("Mouse Y") * 10.0f;

        rotacaoX = clampAngleFPS(rotacaoX, -360, 360);
        rotacaoY = clampAngleFPS(rotacaoY,-80, 80);

        Quaternion xq = Quaternion.AngleAxis(rotacaoX, Vector3.up);
        Quaternion yq = Quaternion.AngleAxis(rotacaoY, -Vector3.right);
        Quaternion q = Quaternion.identity * xq * yq;

        cameraFPS.transform.localRotation = Quaternion.Lerp(cameraFPS.transform.localRotation, q, Time.deltaTime * 10.0f);
    }

    float clampAngleFPS(float angulo, float min, float max){
        if(angulo < -360){
            angulo += 360;
        }
        if(angulo > 360){
            angulo -= 360;
        }
        return Mathf.Clamp(angulo, min, max);
    }

    void OnTriggerEnter(Collider other){
        // detecta colisão
        if(other.gameObject.CompareTag("portal")){
            SceneManager.LoadScene("Fase2");
        }
    }
}