using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour {

	private bool fadeIn;
	private bool fadeOut;
	private bool wait;
	private string typeFade;

	private float segundos;

	// Use this for initialization
	void Start () {
		Color color = GetComponent<Renderer> ().material.color;
		color.a = 0;
		GetComponent<Renderer> ().material.color = color;
		wait = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

	/**
	 * Realiza una trasicion de transparente a opaca y luego al reves
	 */
	private IEnumerator fade(){

		while (fadeIn || fadeOut) { 
			
			float y = 0;
			float aux = (Time.deltaTime * 2) / segundos;

			Color color = GetComponent<Renderer> ().material.color;
			if (fadeIn) {
			
				color.a = Mathf.Min (1.0f, color.a + aux);

			} else {
				color.a = Mathf.Max (0.0f, color.a - aux);
			}
			
			GetComponent<Renderer> ().material.color = color;

			if (color.a == 1) {
				fadeIn = false;
				fadeOut = true;
				yield return new WaitForSeconds (1);
			} else if (color.a == 0) {
				fadeIn = false;
				fadeOut = false;
			} else {
				yield return new WaitForSeconds (Time.deltaTime);
			}
		}
	
	}

	/**
	 * Comienza el fade
	 */
	public void startFade(float time){

		segundos = time;
		fadeIn = true;

		if (fadeIn || fadeOut) {
			StartCoroutine (fade());
		}

	}

}
