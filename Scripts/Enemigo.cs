using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Implementa la lógica que siguen los enemigos (esqueletos)
 */
public class Enemigo : MonoBehaviour {

	public float speed = 0.5f;
	private bool idle = false;
	private bool alive = true;

	// Use this for initialization
	void Start () {
		Controlador.gameover += parar;
		GetComponent<Animator>().SetBool("camina", true);
	}
	
	// Update is called once per frame
	void Update () {
		if (!idle) {
			// Mueve al enemigo hacia la posición del jugador
			transform.position -= transform.forward * Time.deltaTime * speed;
		}

	}

	/**
	 * Mata al enemigo, cambia la animación y lo destruye en 2 segundos. Avisa también al combate.
	 */
	public void disparado(){
		if (!idle && alive) {
			alive = false;
			GetComponent<Animator> ().SetBool ("dead", true);
			Destroy (gameObject, 2);
			idle = true;
			Controlador.controlador.GetComponent<CombateHab> ().enemyKilled ();
			Controlador.gameover -= parar;
		}
	}

	/**
	 * Se comprueba si se colisiona con el jugador, se activa la animación de atacar, se pone el esquleto a estado de reposo y se avisa al ontrolador de la muerte del jugador
	 */
	public void OnTriggerEnter(Collider col){
		if (col.tag == "Head" && !idle) {
			GetComponent<Animator> ().SetTrigger ("cerca");
			idle = true;
			Controlador.gameover -= parar;
			Controlador.controlador.onPlayerDead ();
		}
	}

	/**
	 * Metodo llamado por el evento de la muerte del jugador, para la animación de caminar
	 */
	public void parar(){
		GetComponent<Animator>().SetBool("playerdead", true);
		GetComponent<Animator>().SetBool("camina", false);

		idle = true;
	}
		
	public void OnDisable(){
		Controlador.gameover -= parar;
	}
}
