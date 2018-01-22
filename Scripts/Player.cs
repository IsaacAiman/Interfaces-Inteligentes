using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private bool move = true;
	private bool movimientoAutomatico = false;
	int lastXKnown = 0;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
		if (!movimientoAutomatico) {
			// Parte del movimiento basado en el touchpad de DayDream o en el acelerómetro (para moverse en la z en la demo de que funciona)
			float v = 0;
			float h = 0;

			if (GvrController.IsTouching && move) {
				v = -1 * (GvrController.TouchPos.y - 0.5f);
				h = GvrController.TouchPos.x - 0.5f;
			}
			Transform baseTransform = GameObject.FindWithTag ("MainCamera").transform;
			Vector3 movVer = 2* v * baseTransform.forward;
			Vector3 movHor = 2 *h * baseTransform.right;

			Vector3 final = movVer + movHor;
			final.y = 0;
			GetComponent<Rigidbody> ().velocity = ((final * Time.deltaTime * 80f));
		
		} else {
			if (!(gameObject.transform.localPosition.y < 1.75)) {
				// Si no se ha caído, se mueve en la x y en la z, en la z en funcion del acelerometro
				GetComponent<Rigidbody> ().velocity = new Vector3 (2, 0, Input.acceleration.x);
			}
			if (Controlador.controlador.getMazeInstance ().GetRegion (getCellPosition ()) == 2) {
				// Si se llegó al final del pasillo, se desactiva el movimiento automatico, se frena al personaje y se avisa al controlador, además, se deja moverse a jugador
				GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
				setMoveAutomatico (false);
				Controlador.controlador.musicaFinal ();
				move = true;
			}
			int x = getCellPosition ().x;
			if (x > lastXKnown) {
				Controlador.controlador.GetComponent<FaseFinal> ().update (x);
				lastXKnown = x;
			}
		}

		// Si se detecta que se cae el jugador, se avisa al controlador para que acabe el juego.
		if (gameObject.transform.localPosition.y < -3) {
			StartCoroutine(Controlador.controlador.FinJuego (false));
		}
	}

	public void dontMove(){
		move = false;
	}

	public void moveAgain(){
		move = true;
	}

	// Devuelve la celda actual en la que se encuentra el jugador.
	public Vector2Int getCellPosition(){
		Vector2Int posicion = new Vector2Int ((int)transform.position.x, (int)transform.position.z);

		return posicion;
	}

	public void setMoveAutomatico(bool aux){
		movimientoAutomatico = aux;
	}
}
