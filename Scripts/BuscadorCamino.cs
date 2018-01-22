using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Implementa una búsqueda en anchura para encontrar la habitación 1
 */
public class BuscadorCamino {
	/**
	 * Como su nombre indica, realiza la búsqueda en anchura, es estático porque de nada sirve crear una instancia de la misma
	 */
	public static List<Vector2Int> Buscar(MazeGen laberinto, Vector2Int inicial){
		List<List<Vector2Int>> trayectorias = new List<List<Vector2Int>>();
		List<Vector2Int> visited = new List<Vector2Int> ();

		trayectorias.Add (new List<Vector2Int> ());
		trayectorias [0].Add (inicial);

		while (trayectorias.Count != 0) {
			List<Vector2Int> actual = trayectorias[0];
			if (laberinto.GetRegion (actual [0]) == 1) {
				return actual;
			} else {
				trayectorias.RemoveAt (0);
				foreach(Vector2Int v in laberinto.GetCelda(actual[0]).DireccionesPosibles()){
					List<Vector2Int> aux = new List<Vector2Int> (actual);
					if(!visited.Contains(actual[0] + v)){
						aux.Insert (0, actual [0] + v);
						visited.Add (actual [0] + v);
						trayectorias.Add (aux);
					}
				}
			}
		}

		return null;
	}
}
