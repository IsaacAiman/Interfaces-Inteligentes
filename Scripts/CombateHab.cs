using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Clase que implemnta la lógica de la pelea
 */
public class CombateHab : MonoBehaviour {
	public GameObject smoke;
	public GameObject monster;
	private int enemies_left = 10;
	private MazeGen instancia;
	private Vector3 posPlayer;

	/**
	 * Inicia a corutina e inicializa los campos
	 */
	public void IniciarPelea(MazeGen instancia, Vector3 posPlayer){

		this.posPlayer = posPlayer;
		this.instancia = instancia;
		StartCoroutine (Spawn());
	}

	/**
	 * Elige una celda entre las de la habitación 1 (de los bordes) e invoca el humo y a enemigo en ella.
	 */
	private IEnumerator Spawn(){
		for(int i = 0; i < 10; i++){
			List<Vector2Int> colindantes = instancia.colindantes ();
			MazeCell celda = instancia.GetCelda (colindantes [Random.Range (0, colindantes.Count)]);
			GameObject smoke_ins = Instantiate (smoke, celda.getSuelo ().transform);
			Destroy (smoke_ins, 3);

			yield return new WaitForSeconds (2);
			GameObject enemigo = Instantiate (monster, (celda.getSuelo ().transform));
			Vector3 direction = (enemigo.transform.position - posPlayer).normalized;
			Quaternion rotation = Quaternion.LookRotation(direction);
			enemigo.transform.rotation = rotation;
			enemigo.transform.Rotate (new Vector3 (-enemigo.transform.rotation.eulerAngles.x, 0.0f, 0.0f));
		}
	}

	/**
	 * Decrementa el numero de enemigos restantes, y cuando se llega al final, se avisa al controlador
	 */
	public void enemyKilled(){
		enemies_left--;
		Debug.Log (enemies_left);
		// Pasar a la siguiente fase
		if (enemies_left == 0) {
			Controlador.controlador.fase1Acabada();
		}
	}
}
