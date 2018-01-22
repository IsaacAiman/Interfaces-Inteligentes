using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Flags usado para las direcciones
[System.Flags]
public enum Muros
{
	Nada = 0,
	N = 1,
	S = 2,
	E = 4,
	O = 8,
	Todos = N | S | E | O
};

public class MazeCell{
	public Muros muros = Muros.Todos;		// Por defecto, una casilla tiene todos los muros	
	public float wallWidth = 0.025f;		// Ancho del muro por defecto
	// Gameobject de los distintos muros
	private GameObject muroN = null;
	private GameObject muroS = null;
	private GameObject muroE = null;
	private GameObject muroO = null;
	private GameObject celda = null;
	private bool isConnected = false;
	public Vector2Int coordenadas;
	public GameObject particlePrefabCamino;
	private bool instanciada = false;

	public MazeCell(Vector2Int cords){
		coordenadas = cords;
	}

	/**
	 * Devuelve qué flag corresponde al vector unitario que recibe
	 */
	public static Muros WhichWall(Vector2Int aux){
		if (aux.x == 0) {
			if (aux.y == 1) {
				return Muros.N;
			}
			if (aux.y == -1) {
				return Muros.S;
			}
		}
		if (aux.y == 0) {
			if (aux.x == 1) {
				return Muros.E;
			}
			if (aux.x == -1) {
				return Muros.O;
			}
		}
		return Muros.Nada;
	}

	/**
	 * Devuelve la dirección contraria a la que recibe
	 */
	public static Muros MuroOpuesto(Muros m){
		switch (m) {
		case Muros.N:
			return Muros.S;
		case Muros.S:
			return Muros.N;
		case Muros.E:
			return Muros.O;
		case Muros.O:
			return Muros.E;
		default: 
			return Muros.Nada;
		}
	}

	/**
	 * Devuelve el vector correspondiente a la dirección (MUROS) que recibe
	 */
	public static Vector2Int WhichDir(Muros m){
		switch (m) {
		case Muros.N:
			return new Vector2Int (0, 1);
		case Muros.S:
			return new Vector2Int (0, -1);
		case Muros.E:
			return new Vector2Int (1, 0);
		case Muros.O:
			return new Vector2Int (-1, 0);
		default: 
			return new Vector2Int (0, 0);
		}
	}

	/*
	 * Quita el muro que recibe
	 */
	public void quitarMuro(Muros aux){
		muros = muros & ~aux;
		if (aux == Muros.E && muroE != null) {
			GameObject.Destroy (muroE);
		} else if (aux == Muros.O && muroO != null){
			GameObject.Destroy (muroO);
		} else if (aux == Muros.N && muroN != null){
			GameObject.Destroy (muroN);
		} else if (aux == Muros.S && muroS != null){
			GameObject.Destroy (muroS);
		}
	}

	/*
	 * Añade el muro que recibe
	 */
	public void ponerMuro(Muros aux){
		muros = muros | aux;
	}

	/**
	 * Instancia la celda, es decir, instancia el suelo y los gameobjects llamando a los metodos correspondientes
	 */
	public void instantiate(float x, float z, Transform transform, Material mat, Material suelo){
		GameObject newCell = (GameObject)GameObject.Instantiate(Resources.Load("MazeCell", typeof(GameObject)));
		newCell.name = "" + (x - 0.5f) + "," + (z - 0.5f);
		newCell.transform.parent = transform;
		newCell.transform.localPosition = new Vector3(x, 0f, z);
		newCell.GetComponentInChildren<Renderer> ().material = suelo;
		celda = newCell;
		ponerMuros (x, z, newCell.transform, mat);
		instanciada = true;
	}

	/**
	 * Instancia los muros de una celda basado en los flags, y los pone como hijos del tranform que recibe, así como asigna el material que recibe
	 */
	public void ponerMuros(float x, float z, Transform transform, Material mat){
		mat.mainTexture.wrapMode = TextureWrapMode.Repeat;
		if ((Muros.N & muros) == Muros.N) {
			GameObject muro = (GameObject)GameObject.Instantiate(Resources.Load("Wall", typeof(GameObject)));
			muro.name = "N";
			muro.transform.parent = transform;
			muro.transform.localPosition = new Vector3(0f, 1f, 0.5f - wallWidth/2);
			muro.GetComponentInChildren<Renderer> ().material = mat;
			muroN = muro;
		}
		if ((Muros.S & muros) == Muros.S) {
			GameObject muro = (GameObject)GameObject.Instantiate(Resources.Load("Wall", typeof(GameObject)));
			muro.name = "S";
			muro.transform.parent = transform;
			muro.transform.localPosition = new Vector3(0f, 1f,- (0.5f - wallWidth/2));
			muro.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
			muro.GetComponentInChildren<Renderer> ().material = mat;
			muroS = muro;
		}
		if ((Muros.E & muros) == Muros.E) {
			GameObject muro = (GameObject)GameObject.Instantiate(Resources.Load("Wall", typeof(GameObject)));
			muro.name = "E";
			muro.transform.parent = transform;
			muro.transform.localPosition = new Vector3(0.5f - wallWidth/2, 1f, 0f);
			muro.transform.rotation = Quaternion.Euler (new Vector3 (0, 90, 0));
			muro.GetComponentInChildren<Renderer> ().material = mat;
			muroE = muro;
		}
		if ((Muros.O & muros) == Muros.O) {
			GameObject muro = (GameObject)GameObject.Instantiate(Resources.Load("Wall", typeof(GameObject)));
			muro.name = "O";
			muro.transform.parent = transform;
			muro.transform.localPosition = new Vector3(- (0.5f - wallWidth/2), 1f, 0f );
			muro.transform.rotation = Quaternion.Euler (new Vector3 (0, 270, 0));
			muro.GetComponentInChildren<Renderer> ().material = mat;
			muroO = muro;
		}
	}

	/**
	 * Devuelve cuántos pasillos le faltan a la celda
	 */
	public int NumeroPasillos(){
		Muros aux = muros;
		int count = 0;
		while (aux != 0) {
			aux = aux & (aux - 1);
			count++;
		}

		return 4 - count;
	}

	/**
	 * Devuelve las direcciones hacia las que puede moverse desde la celda
	 */
	public List<Vector2Int> DireccionesPosibles(){
		List<Vector2Int> aux = new List<Vector2Int> ();
		if (Muros.Nada == (muros & Muros.N)) {
			aux.Add(WhichDir(Muros.N));
		}
		if (Muros.Nada == (muros & Muros.S)) {
			aux.Add(WhichDir(Muros.S));
		}
		if (Muros.Nada == (muros & Muros.E)) {
			aux.Add(WhichDir(Muros.E));
		}
		if (Muros.Nada == (muros & Muros.O)) {
			aux.Add(WhichDir(Muros.O));
		}
		return aux;
	}

	public bool isIntocable(){
		return isConnected;
	}

	public void setConnected(){
		isConnected = true;;
	}

	public GameObject getSuelo(){
		return celda;
	}


}
