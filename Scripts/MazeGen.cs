using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MazeGen : MonoBehaviour {

	public int sizeX, sizeZ, distpasillo;		// Tamaño del laberinto
	public int roomSize;			// Tamaño de las habitaciones

	private MazeCell[,] cells;		// Matriz con las casillas del laberinto
	private int[,] regiones;		// Region para cada casilla

	private Vector2Int habitacion1;	// Coordenadas de la esquina de la habitacion 1 
	private Vector2Int habitacion2;	// Coordenadas de la esquina de la habitacion 2
	private int zpasillo;

	private List<Vector2Int> bordesHab1 = new List<Vector2Int>();

	public Material[] materiales;	// Materiales para los distintos tipos de habitacion
	public Material[] suelos;
	public double probRecto;		// Probabilidad en la generacion del laberinto de ir seguir en la misma direccion si es posible
	public double probLimpiar;		// Probabilidad de un final muerto de ser limpiado

	/**
	 * Método que crea el laberinto (Va haciendo las medidas necesarias)
	 */
	public Vector2Int Generate () {
		// Puesta a punto de las matrices de celdas
		cells = new MazeCell[sizeX, sizeZ];
		regiones = new int[sizeX, sizeZ];

		for (int i = 0; i < sizeX; i++) {
			for (int z = 0; z < sizeZ; z++) {
				cells [i, z] = new MazeCell (new Vector2Int(i, z));
			}
		}

		// Generacion de habitaciones
		GenerateRooms ();

		// Generación del pasillo que las une
		GenerarPasilloHabitaciones ();

		//Generacion del laberinto
		GenerarLaberinto();

		//Crear el muro que separa la habitación dos del pasillo que la une con la habitación 2.
		muroPasillo(true);

		// Crea las celdas (instancia los gameobjects el metodo invocado)
		for(int x = 0; x < sizeX; x++){
			for(int y = 0; y < sizeZ; y++){
				if (regiones [x, y] != 0)
					CreateCell(x,y,regiones[x,y] - 1);
			}
		}


		// Instanciamos el gameobject de la llave y lo configuramos 
		GameObject llave = (GameObject)GameObject.Instantiate(Resources.Load("key_gold", typeof(GameObject)));
		llave.tag = "Llave";
		llave.transform.localPosition = new Vector3 (habitacion1.x + ((float)roomSize / 2), 0, habitacion1.y + (float)roomSize / 2);

		GameObject cofre = (GameObject)GameObject.Instantiate(Resources.Load("Cofre", typeof(GameObject)));
		cofre.transform.localPosition = new Vector3 (habitacion2.x + ((float)roomSize / 2), 0, habitacion2.y + (float)roomSize / 2);

		for (int i = 0; i < sizeX; i++) {
			for (int z = 0; z < sizeZ; z++){
				if (regiones [i, z] == 4) {
					return new Vector2Int (i, z);
				}
			}
		}


		return new Vector2Int (0, 0);
	}

	/**
	 * Instancia la celda en la posicion recibida con el material correspondiente
	 */
	private void CreateCell (int x, int z, int region) {
		cells [x, z].instantiate (x + 0.5f, z + 0.5f, transform, materiales[region], suelos[region]);
	}

	/**
	 * Genera aleatoriamente la posicion de las habitaciones con una distancia entre ellas mínima
	 */
	private void GenerateRooms(){
		habitacion1.x = Random.Range (5, (sizeX/2 - roomSize) - 3);
		habitacion1.y = habitacion2.y = Random.Range (3, sizeZ - roomSize - 1);
		habitacion2.x = habitacion1.x + roomSize + distpasillo;

		MurosHabitaciones (1, habitacion1, roomSize, roomSize);
		MurosHabitaciones (2, habitacion2, roomSize, roomSize);
	}

	/**
	 * Método que crea un area rectangular vacía rodeada por muros del tipo deseado
	 */
	private void MurosHabitaciones(int region, Vector2Int coordenadas, int distx, int distz){
		for (int x = coordenadas.x; x < coordenadas.x + distx; x++) {
			for (int z = coordenadas.y; z < coordenadas.y + distz; z++) {
				cells [x, z].muros = Muros.Nada;
				regiones [x, z] = region;
			}
		}

		for (int i = 0; i < distx; i++) {
			if (region == 1) {
				bordesHab1.Add (new Vector2Int (coordenadas.x + i, coordenadas.y));
				bordesHab1.Add (new Vector2Int (coordenadas.x + i, coordenadas.y + distz - 1));
			}
			cells [coordenadas.x + i, coordenadas.y].ponerMuro (Muros.S);
			cells [coordenadas.x + i, coordenadas.y + distz - 1].ponerMuro (Muros.N);
		}
		for (int i = 0; i < distz; i++) {
			if (region == 1) {
				bordesHab1.Add (new Vector2Int (coordenadas.x, coordenadas.y + i));
				bordesHab1.Add (new Vector2Int (coordenadas.x + distx - 1, coordenadas.y + i));
			}
			cells [coordenadas.x, coordenadas.y + i].ponerMuro (Muros.O);
			cells [coordenadas.x + distx - 1, coordenadas.y + i].ponerMuro (Muros.E);
		}
	}

	/**
	 * Método encargado de crear el pasillo entre las habitaciones 
	 */
	private void GenerarPasilloHabitaciones(){
		zpasillo = Random.Range (habitacion1.y, habitacion1.y + roomSize - 3);
		int distXgiro = Random.Range (3, Mathf.Abs (habitacion1.x - habitacion2.x) - roomSize - 3);

		//Parte que sale de la habitacion 1 
		MurosHabitaciones(3, new Vector2Int(habitacion1.x + roomSize, zpasillo), distpasillo, 3);
		for (int i = 0; i < 3; i++) {
			cells [habitacion1.x + roomSize - 1, zpasillo + i].quitarMuro (Muros.E);
			cells [habitacion1.x + roomSize , zpasillo + i].quitarMuro (Muros.O);
			cells [habitacion2.x, zpasillo + i].quitarMuro (Muros.O);
			cells [habitacion2.x - 1, zpasillo + i].quitarMuro (Muros.E);
		}
	}

	/**
	 * Método que se ocupa de la parte principal de la creación del laberinto
	 */
	private void GenerarLaberinto(){

		Stack pila = new Stack();

		List<KeyValuePair<Vector2Int, Vector2Int>> conexionHabitacion = new List<KeyValuePair<Vector2Int, Vector2Int>> ();		// Candidatos para la conexión
		List<Vector2Int> ends = new List<Vector2Int> ();																	// Puntos finales de pasillos

		pila.Push (new Vector2Int(0,0));

		Vector2Int actual;
		Vector2Int selected = new Vector2Int (0, 0);
		while (pila.Count != 0) {
			actual = (Vector2Int) pila.Pop ();
			regiones [actual.x, actual.y] = 4;

			selected = SeleccionCandidatos (actual, conexionHabitacion, selected);
			if (selected.x != 0 || selected.y != 0) {
				Vector2Int aux = selected + actual;
				pila.Push (actual);
				pila.Push (aux);
				cells [actual.x, actual.y].quitarMuro (MazeCell.WhichWall (selected));
				cells [aux.x, aux.y].quitarMuro (MazeCell.WhichWall (selected *-1));
			} else {
				if (cells [actual.x, actual.y].NumeroPasillos () < 2) {
					ends.Add (actual);
				}
			}
			regiones[actual.x,actual.y] = 4;
		}

		// Parte de la conexión con la habitación 1
		foreach (KeyValuePair<Vector2Int, Vector2Int> conexionHab1 in conexionHabitacion) {
			if (Random.RandomRange (0f, 1f) > 0.7) {
				cells [conexionHab1.Key.x, conexionHab1.Key.y].quitarMuro (MazeCell.WhichWall(conexionHab1.Value));
				Vector2Int celdaHab = conexionHab1.Key + conexionHab1.Value;
				cells [celdaHab.x, celdaHab.y].quitarMuro (MazeCell.WhichWall (conexionHab1.Value * -1));
				cells [conexionHab1.Key.x, conexionHab1.Key.y].setConnected ();
			}
		}
		limpiarPasillos (probLimpiar, ends);
	}

	/**
	 * Busca las celdas adyacentes a la dada en la que se puede continuar el pasillo y elige una de ellas aleatoriamente, con la posibilidad de elegir la última usada directamente
	 */
	private Vector2Int SeleccionCandidatos(Vector2Int actual, List<KeyValuePair<Vector2Int, Vector2Int>> conexionHabitacion, Vector2Int lastDir){
		Vector2Int[] coords = new Vector2Int[4];
		List<Vector2Int> validos = new List<Vector2Int>();

		coords[0] = actual + new Vector2Int (1, 0);
		coords[1]= actual + new Vector2Int (-1, 0);
		coords[2]= actual + new Vector2Int (0, 1);
		coords[3]= actual + new Vector2Int (0, -1);

		for (int i = 0; i < 4; i++){
			validos.Add (coords[i]);
			if (coords [i].x < 0 || coords [i].y < 0 || coords [i].x >= sizeX || coords [i].y >= sizeZ) {
				validos.Remove (coords[i]);
			}

			else if (regiones [coords [i].x, coords [i].y] != 0) {
				if(regiones [coords [i].x, coords [i].y] == 1){
					conexionHabitacion.Add(new KeyValuePair<Vector2Int, Vector2Int>(actual, coords[i] - actual));
				}
				validos.Remove (coords[i]);
			}
		}

		if (validos.Count > 0) {
			Vector2Int candidato = actual + lastDir;
			foreach (Vector2Int v in validos) {
				if (v.x == candidato.x && v.y == candidato.y && Random.Range (0f, 1f) <= probRecto) {
					return lastDir;
				}
			}
			return validos[Random.Range (0, validos.Count)] - actual;
		}
		return new Vector2Int(0,0);
	}

	/**
	 * Se encarga de seleccionar qué pasillos serán limpiados de aquellos puntos finales que recibe
	 */
	private void limpiarPasillos(double prob, List<Vector2Int> ends){
		foreach(Vector2Int v in ends){
			if (Random.Range (0f, 1f) < prob) {
				limpiarPasillo (v);
			}
		}
	}

	/**
	 * Limpia el pasillo que lleva a la celda seleccionada hasta que se encuentra una bifurcación
	 */
	private void limpiarPasillo(Vector2Int principio){
		Vector2Int current = principio;
		Muros lastDir = Muros.Nada;

		int aux = 0;
		while (cells [current.x, current.y].NumeroPasillos () < 3 && !cells[current.x, current.y].isIntocable()) {
			regiones [current.x, current.y] = 0;
			lastDir = direccion (lastDir, cells [current.x, current.y]);
			current = current + MazeCell.WhichDir (lastDir);
			aux++;
		}

		cells [current.x, current.y].ponerMuro (MazeCell.MuroOpuesto (lastDir));
	}

	/**
	 * Busca qué dirección seguir en la limpieza de los muros (el pasillo muro abierto que no sea el que se le pasa)
	 */
	private Muros direccion(Muros lastDir, MazeCell mc){
		List<Muros> aux = new List<Muros> ();
		aux.Add (Muros.N); aux.Add (Muros.S); aux.Add (Muros.O); aux.Add (Muros.E);
		foreach (Muros m in aux){
			if ((MazeCell.MuroOpuesto(lastDir) != m) && (Muros.Nada == (mc.muros & m))){
				return m;
			}
		}
		return Muros.Nada;
	}

	// Devuelve el conjunto de celdas de la región especificada.
	private List<MazeCell> getRegionCells(int numeroRegion){
		List<MazeCell> celdas = new List<MazeCell>();

		for (int i = 0; i < sizeX; i++) {
			for (int z = 0; z < sizeZ; z++) {
				if (regiones [i, z] == numeroRegion) {
					celdas.Add(cells [i, z]);
				}
			}
		}

		return celdas;
	}

	// Genera o quita los que unen la habitación 1 con el pasillo.
	public void muroPasillo(bool poner){
		List<MazeCell> celdas = getRegionCells (3);
		int widthPasillo = 3;

		MazeCell celda;

		for (int i = 0; i < widthPasillo; i++) {
			
			int x = celdas[i].coordenadas.x - 1;
			int z = celdas[i].coordenadas.y;
			celda = cells [x, z];
			if (poner) {
				celda.ponerMuro (Muros.E);
			} else {
				celda.quitarMuro (Muros.E);
			}
		}
	}

	public MazeCell GetCelda(Vector2Int coord){
		return cells [coord.x, coord.y];
	}

	public int GetRegion(Vector2Int coord){
		return regiones [coord.x, coord.y];
	}

	/**
	 * Devuelve los bordes de la habitacion 1
	 */
	public List<Vector2Int> colindantes(){
		return bordesHab1;
	}


	/**
	 * Devuelve el punto central de la habitación 1
	 */
	public Vector2 getMiddleHab1(){
		return new Vector2 (habitacion1.x + (float)roomSize/2, habitacion1.y + (float)roomSize/2);
	}

	/**
	 * Devuelve el punto inicial del pasillo final, centrado en la z
	 */
	public Vector2 getStartPasillo(){
		return new Vector2 (habitacion1.x + roomSize - 1, zpasillo + 1.5f);
	}

	public int getZPasillo(){
		return zpasillo;
	}
}