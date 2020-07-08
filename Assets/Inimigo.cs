using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Inimigo : MonoBehaviour
{
    private Vector3 target1;
    private Vector3 target2;
    private float speed = 3;
    private Vector3 positionOld = new Vector3();
    public GameObject jogador;
    private static bool primeiro = true;
    private static AudioSource hit;
    private static AudioSource death;

    void Start()
    {
        hit = GetComponents<AudioSource>()[0];
        death = GetComponents<AudioSource>()[1];

        target2 = new Vector3(Random.Range(-10,10), 3.62f, Random.Range(-10,10));

        if(primeiro){
            primeiro = false;
            for(int i=0; i<10; i++){

                Vector3 posicao = new Vector3(Random.Range(-22,-6),3.62f,Random.Range(-3,2));
                GameObject outro = Instantiate(gameObject, posicao, transform.rotation) as GameObject;
            }
        }
    }
    void Update()
    {
        target2 = new Vector3(jogador.transform.position.x,3.62f, jogador.transform.position.z);
        // salva a posição anterior caso a colisão aconteça
        positionOld = transform.position;
        // se estiver mais de 10 de de istancia do jogador, segue aleatoriamente
        Vector3 target;
        if(Vector3.Distance(transform.position, target2) > 10){
            target = target1;  // aleatorio
        } else {
            target = target2; // jogador
        }
        // verifica se o inimigo encontrou seu target
        float dist = Vector3.Distance(transform.position, target);
        if(dist <= 0.1){
            target1 = new Vector3 (Random.Range(-10,10),3.62f,Random.Range(-10,10));
        }
        // move o inimigo
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);
    }
    void OnCollisionEnter(Collision collision){
        // detecta colisão
        transform.position = positionOld;
        target1 = new Vector3 (Random.Range(-10,10),3.62f,Random.Range(-10,10));
        if(collision.gameObject.CompareTag("jogador")){
            hit.Play();
            Controller.vidas--;
            if(Controller.vidas <= 0){
                SceneManager.LoadScene("GameOver");
            }
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("tiro")){
            death.Play();
            Destroy(gameObject);
        }
    }
}