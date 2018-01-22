using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Clase encargada de la lógica que lleva el baúl de la habitación final
 */
public class CofreScript : MonoBehaviour {
	private bool used = false;	//Sólo se puede cerrar una vez
	public GameObject llave;    // Llave que poner en el interior 

	/**
	 * Método llamado por el evento pointerClick, pone la llave en el cofre
	 */
	public void clickado(){
		if (!used) {
			GameObject instanciaLlave = Instantiate (llave, gameObject.transform);
			instanciaLlave.transform.rotation = Quaternion.Euler (+90, 0, 0);
			instanciaLlave.transform.localScale = new Vector3 (0.35f, 0.35f, 0.35f); 
			instanciaLlave.transform.localPosition = new Vector3 (-1, 0, 0.5f);
			used = true;
			Invoke ("cerrar", 1.5f);
		}
	}

	/**
	 * Cierra el baúl y llama a finalizar el juego
	 */
	private void cerrar(){
		gameObject.GetComponent<Animator> ().SetTrigger ("Activate");
		StartCoroutine (Controlador.controlador.FinJuego (true));	
	
	}
}
