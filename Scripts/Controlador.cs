using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Clase que coordina los principales eventos que suceden en el juego
 */
public class Controlador : MonoBehaviour {
	// Eventos sobre la muerte del jugador en manos de enemigos
	public delegate void Muerte();
	public static event Muerte gameover;

	//Patron singleton
	public static Controlador controlador;

	//Componentes necesarios
	public MazeGen mazePrefab;
	private MazeGen mazeInstance;
	public AudioClip cancionInicio;
	public AudioClip cancionPelea;
	public AudioClip cancionFin;
	public AudioSource audioSource;
	private GameObject player;
	private GameObject combate;
	public GameObject particulasCamino;

	//Usada para no invocar más de una vez las acciones que terminan el juego.
	private bool aunJugando = true;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = cancionInicio;
		audioSource.Play ();

		mazeInstance = Instantiate (mazePrefab) as MazeGen;
		Vector2Int startPos = mazeInstance.Generate();
		Debug.Log ("StartPos: " + startPos);

		player = GameObject.Find ("Head");
		player.transform.localPosition = new Vector3 (startPos.x +0.5f, 1.5f, startPos.y + 0.5f);
	}
	
	// Update is called once per frame
	void Update () {


	}

	void Awake(){
		if (controlador == null) {
			controlador = this;
			DontDestroyOnLoad (this);
		} else if (controlador != this) {
			Destroy (gameObject);
		}
	}

	public void keyPicked(){
		audioSource.clip = cancionPelea;
		audioSource.PlayDelayed (2.0f);

		GameObject.Find ("Fade").GetComponent<Fade> ().startFade (2);
		StartCoroutine (interfase1 ());
	}

	// Muestra el camino hacia la llave
	public void mostrarCamino(){
		StartCoroutine (mostrarCaminoSecuencial ());
	}

	/**
	 * Método encargado de mostrar las particulas que indican el camino
	 */
	private IEnumerator mostrarCaminoSecuencial(){
		List<Vector2Int> caminoAMostrar = BuscadorCamino.Buscar (getMazeInstance(), player.GetComponent<Player> ().getCellPosition ());
		caminoAMostrar.Reverse ();
		for(int i = 0; i < 10 && i < caminoAMostrar.Count; i++){
			Quaternion rotacion = Quaternion.Euler (-90, 0, 0);
			Vector2Int v = caminoAMostrar [i];
			Vector3 posicion = new Vector3 (v.x + 0.5f , 0.0f, v.y + 0.5f);
			GameObject aux = Instantiate (particulasCamino, posicion, rotacion);
			Destroy (aux, 3);
			yield return new WaitForSeconds (1f);
		}
	}

	public MazeGen getMazeInstance(){
		return mazeInstance;
	}

	/**
	 * Interfase 1, coordina los eventos desde que se coge la llave hasta que salen los enemigos
	 */
	private IEnumerator interfase1(){

		yield return new WaitForSeconds (1.5f);
		Vector2 aux = mazeInstance.getMiddleHab1 ();
		player.transform.localPosition = new Vector3 (aux.x, 1.5f, aux.y);
		player.GetComponent<Player> ().dontMove();
		yield return new WaitForSeconds (2);
		GetComponent<CombateHab> ().IniciarPelea (mazeInstance, player.transform.localPosition);
	}


	/**
	 * Inicia el evento que para a los enemigos y llama a finalizar el juego
	 */
	public void onPlayerDead(){
		gameover ();
		GetComponent<CombateHab> ().StopAllCoroutines ();

		StartCoroutine (FinJuego (false));
	}

	/**
	 * Termina la fase 1, llama a fade, y llama a interfase 2
	 */
	public void fase1Acabada(){
		GameObject.Find ("Fade").GetComponent<Fade> ().startFade (2);
		StartCoroutine (interfase2 ());
	}

	/**
	 * Interfase 2, desde que se temrina el combate hasta que arranca el mov. automatico
	 */
	private IEnumerator interfase2(){
		yield return new WaitForSeconds (1.5f);
		Vector2 aux = mazeInstance.getStartPasillo ();
		player.transform.localPosition = new Vector3 (aux.x, 1.83f, aux.y);
		player.GetComponent<Player> ().dontMove();
		mazeInstance.muroPasillo (false);
		GameObject.FindWithTag ("Luz").GetComponent<Light> ().intensity = 1;
		yield return new WaitForSeconds (0.5f);
		float camRot = GameObject.FindWithTag ("MainCamera").transform.rotation.eulerAngles.y;
		GameObject.FindWithTag ("Head").transform.rotation = Quaternion.Euler (0, 90 -camRot, 0);
		yield return new WaitForSeconds (1.5f);
		GetComponent<FaseFinal> ().iniciarFase (mazeInstance);
	}

	/**
	 * Cambia la música
	 */
	public void musicaFinal(){
		
		audioSource.clip = cancionFin;
		audioSource.PlayDelayed (1.0f);
	}

	/**
	 * Lleva a cabo las acciones para indicar al personaje que el juego ha terminado, 
	 * con resultado satisfactorio o no en funcion del parametro
	 */
	public IEnumerator FinJuego(bool ganar){
		if (aunJugando) {
			aunJugando = false;
			yield return new WaitForSeconds (1f);
			GameObject.Find ("Fade").GetComponent<Fade> ().startFade (2);
			yield return new WaitForSeconds (1.5f);
			musicaFinal ();
			GameObject.FindGameObjectWithTag ("Luz").GetComponent<Light> ().intensity = 1;


			player.GetComponent<Player> ().dontMove ();
			player.transform.localPosition = new Vector3 (0, 25, 0);
			if (ganar) {
				GameObject.Instantiate (Resources.Load ("CreditosFinales", typeof(GameObject)));
			} else {
				GameObject.Instantiate (Resources.Load ("GameOver", typeof(GameObject)));
			}

			player.GetComponent<Rigidbody> ().useGravity = false;
			player.GetComponent<Player> ().setMoveAutomatico (false);
		}
	}
		
}
