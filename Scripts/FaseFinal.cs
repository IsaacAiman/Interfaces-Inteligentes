using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Aquí encontramos la lógica de la fase final, donde se cae el suelo del pasillo principal
 */
public class FaseFinal : MonoBehaviour {

	public Material mat;  // Material de las celdas que se van a caer
	private int[] next = new int[2]; //Coordenadas en la z de las celdas que caerán (de 0 a 2)
	private MazeGen instancia; 

	/**
	 * Desactiva el movimiento del jugador
	 */
	public void iniciarFase(MazeGen instancia){
		GameObject.Find ("Head").GetComponent<Player> ().setMoveAutomatico (true);
		this.instancia = instancia;
	}

	/**
	 * Llamado cuando el jugador cambia de celda en la x,
	 * comprueba si debe activarse la caía de las celdas marcadas o preparar para ello a nuevas celdas, 
	 * se caerán las que estén en coordenadas de x divisibles entre 4.
	 */
	public void update(int x){
		if(x + 2 < instancia.getStartPasillo().x + instancia.distpasillo){
			if ((x + 4) % 4 == 0 && x + 4 < instancia.getStartPasillo().x + instancia.distpasillo) {
				next [0] = Random.Range (0, 3);
				do {
					next[1] = Random.Range(0,3);
				} while(next [0] == next [1]);

				instancia.GetCelda (new Vector2Int (x + 4, instancia.getZPasillo () + 
					next [0])).getSuelo ().GetComponentInChildren<Renderer> ().material = mat;
				instancia.GetCelda (new Vector2Int (x + 4, instancia.getZPasillo () + 
					next [1])).getSuelo ().GetComponentInChildren<Renderer> ().material = mat;

			} else if ((x + 2) % 4 == 0){
				GameObject g1 = instancia.GetCelda (new Vector2Int (x + 2, instancia.getZPasillo () +
				                next [0])).getSuelo ().transform.GetChild (0).gameObject;
				GameObject g2 = instancia.GetCelda (new Vector2Int (x + 2, instancia.getZPasillo () +
								next [1])).getSuelo ().transform.GetChild (0).gameObject;
				Destroy (g1, 2);
				Destroy (g2, 2);
				g1.AddComponent<Rigidbody> ();
				g2.AddComponent<Rigidbody> ();
			}
		}
	}
}
