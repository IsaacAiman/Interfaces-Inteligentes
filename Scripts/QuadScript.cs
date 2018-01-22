using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadScript : MonoBehaviour {
	// Se avisa al controlador para que muestre el camino
	public void clicked(){
		GameObject.FindWithTag ("Controlador").GetComponent<Controlador> ().mostrarCamino ();
	}
}
