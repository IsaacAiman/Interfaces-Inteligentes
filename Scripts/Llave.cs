using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Llave : MonoBehaviour {

	private float speed = 10f;
	public GameObject particlePrefab;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up, speed * Time.deltaTime);
	}

	/**
	 * Se comprueba si colisiona con el jugador para avisar al controlador de ello y por tanto iniciar la interfase 1
	 */
	void OnCollisionEnter(Collision collider){
		if (collider.collider.tag == "Head") {
			GameObject aux = Instantiate (particlePrefab, transform.localPosition, transform.rotation);
			Destroy (gameObject);
			Destroy (aux, 2);
			Controlador.controlador.keyPicked ();
		}
	}
}
