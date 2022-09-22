using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AI : MonoBehaviour {

    private GameController gc;
    private List<N_Grama> nGramaArray;
    private int stringPosition;
    private float[] bestProbability;
    private float n3BestOpt, n2BestOpt;
    public int minimunPredictionData;

	// Use this for initialization
	void Start ()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        nGramaArray = new List<N_Grama>();
        stringPosition = -1;
    }

    public void Play()
    {
        string actions = gc.lastActions;
        float probN3 = 0;
        float probN2 = 0;

        //comprobamos si el string tiene 3 caracteres
        if (actions.Length == 3)
        {
            //Se comprueba si el string de las ultimas acciones existe, sino se crea un index nuevo en el array de datos
            if (CompareString(actions))
            {   
                //comprobamos que haya suficientes datos anteriores para realizar una prediccion
                if (nGramaArray[GetIndex(actions)].getTotal() > minimunPredictionData)
                {
                    //guardamos la mejor opción del nGrama de 3 caracteres y guardamos su probabilidad
                    n3BestOpt = getBestOptionInArray(GetIndex(actions));
                    probN3 = nGramaArray[GetIndex(actions)].GetProbability((int)getBestOptionInArray(GetIndex(actions))) / nGramaArray[GetIndex(actions)].getTotal();
                }
                else
                    //si no hay datos suficientes devolvemos -1 como mejor probabilidad
                    n3BestOpt = -1;
            }
            else
            {
                //aquí se crea el Ngrama vacio y se da como mejor opcion -1 dado que no contiene datos aun
                AddNGram(actions);
                n3BestOpt = -1;
            }
        }
        else
        {
            //en caso de que la cadena de texto no tenga 3 caracteres devolvemos -1
            n3BestOpt = -1;
        }

        //cortamos el texto para el NGrama de 2 mediante substring()
        if (actions.Length == 3)
        {
            actions = actions.Substring(1, 2);
        } 

        //comprobamos si el string tiene 2 char.
        if (actions.Length == 2)
        {
            //Se comprueba si el string de las ultimas acciones existe, sino se crea un index nuevo en el array de datos
            if (CompareString(actions))
            {
                //comprobamos que haya suficientes datos anteriores para realizar una prediccion
                if (nGramaArray[GetIndex(actions)].getTotal() > minimunPredictionData)
                {
                    //guardamos la mejor opción del nGrama de 2 caracteres y guardamos su probabilidad
                    n2BestOpt = getBestOptionInArray(GetIndex(actions));
                    probN2 = nGramaArray[GetIndex(actions)].GetProbability((int)getBestOptionInArray(GetIndex(actions))) / nGramaArray[GetIndex(actions)].getTotal();
                }
                else
                    //si no hay datos suficientes devolvemos -1 como mejor probabilidad
                    n2BestOpt = -1;
            }
            else
            {
                //aquí se crea el Ngrama vacio y se da como mejor opcion -1 dado que no contiene datos aun
                AddNGram(actions);
                n2BestOpt = -1;
            }
        }
        else
        {
            //en caso de que la cadena de texto no tenga 2 caracteres devolvemos -1
            n2BestOpt = -1;
        }

        //si ninguna de las dos cadenas es optima se hace aleatorio
        if (n3BestOpt == -1 && n2BestOpt == -1)
        {
            SelectRand();
        }

        // si alguna es optima, lo seleccionamos
        else if (n3BestOpt == -1 || n2BestOpt == -1)
        {
            if (n3BestOpt == -1)
            {
                string choice = WinningMove(ConvertToChoice(n2BestOpt));
                gc.setPlayer2(choice);
            //    Debug.Log("Mi prediccion es: " + choice + " con una probabilidad de: " + probN2);
            }
            else
            {
                string choice = WinningMove(ConvertToChoice(n3BestOpt));
                gc.setPlayer2(choice);
            //    Debug.Log("Mi prediccion es: " + choice + " con una probabilidad de: " + probN3);
            }

        }

        //si ambos son optimos, ponderamos cuál pesa más.
        else if (n3BestOpt != -1 && n2BestOpt != -1)
        {
            float _probN3 = probN3 * 0.6f;
            float _probN2 = probN2 * 0.4f;

            if (_probN2 == _probN3)
            {
                if (Rand(0, 2) == 0)
                {
                    gc.setPlayer2(WinningMove(ConvertToChoice(n3BestOpt)));
               //     Debug.Log("Mis Ngramas tienen la misma probabilidad asi que aleatoriamente he seleccionado entre los dos: " + gc.getPlayer2() + " Con una probabilidad de: " + probN3);
                }
                else
                {
                    gc.setPlayer2(WinningMove(ConvertToChoice(n2BestOpt)));
                //    Debug.Log("Mis Ngramas tienen la misma probabilidad asi que aleatoriamente he seleccionado entre los dos: " + gc.getPlayer2() + " Con una probabilidad de: " + probN2);
                }
            }
            else if (_probN3 > _probN2)
            {
                gc.setPlayer2(WinningMove(ConvertToChoice(n3BestOpt)));
             //   Debug.Log("Tengo varios datos, pero mi prediccion de NGrama(3) pesa más y selecciono: " + gc.getPlayer2() + " con una probabilidad de: " + probN3);
            }
            else
            {
                gc.setPlayer2(WinningMove(ConvertToChoice(n2BestOpt)));
               // Debug.Log("Tengo varios datos, pero mi prediccion de NGrama(2) pesa más y selecciono: " + gc.getPlayer2() + " con una probabilidad de: " + probN2);
            }
        }

        //añadimos los datos probabilísticos al NGrama
        if (GetIndex(gc.lastActions) != -1)
        {
            Debug.Log("Añado los datos del Ngrama(3)");
            AddData(gc.lastActions);
        }
        if (GetIndex(actions) != -1)
        {
            Debug.Log("Añado los datos del Ngrama(2)");
            AddData(actions);
        }

        gc.Winner();
    }

    public void AddNGram (string actions)
    {
        nGramaArray.Add(new  N_Grama(actions, new float[4] { 0, 0, 0, 0 }));
    }

    public int GetIndex(string cadena)
    {

        for (int i = 0; i < nGramaArray.Count; i++)
        {
            if (cadena == nGramaArray[i].GetCombination())
            {
                //Aquí se recogen las probs de las cadenas de texto.
                return i;
            }
        }

        return -1;
    }

    //Esta función añade los datos del jugador al array de N_Gramas
    public void AddData(string actions)
    {
        float[] data = new float[4];
        if (gc.getPlayer1() == "R")
        {
            data = new float[4] { 1, 0, 0, 0 };
        }
        else if (gc.getPlayer1() == "P")
        {
            data = new float[4] { 0, 1, 0, 0 };
        }
        else if (gc.getPlayer1() == "S")
        {
            data = new float[4] { 0, 0, 1, 0 };
        }
        if (GetIndex(actions) != -1)
        {
            nGramaArray[GetIndex(actions)].SetProbability(data);
            Debug.Log("Mi cadena es " + actions + " Y sus probabilidades son: R " + nGramaArray[GetIndex(actions)].GetProbability(0) 
                                                                         + " P " + nGramaArray[GetIndex(actions)].GetProbability(1) 
                                                                         + " S " + nGramaArray[GetIndex(actions)].GetProbability(2)
                                                                         + " Total " + nGramaArray[GetIndex(actions)].GetProbability(3));
        }
        else
            Debug.Log("Array fuera de rango");
    }

    //Compara nuestro string con lso strings creados, si existe nos devuelve la posicion de este, si no, 
    //lo crea y nos devuelve la posicion en la que se ha creado.
    public bool CompareString(string lastComands)
    {
        if (lastComands.Length < 2)
        {
            return false;
        }
        else if (lastComands.Length == 3)
        {
            for (int i = 0; i < nGramaArray.Count; i++)
            {
                if (lastComands == nGramaArray[i].GetCombination())
                {
                    //Aquí se recogen las probs de las cadenas de texto.
                    //Debug.Log("He encontrado la cadena de texto y su posicion es " + i + " y su valor es " + nGramaArray[i].GetCombination());
                    stringPosition = i;
                    return true;
                }
            }
        }
        else if (lastComands.Length == 2)
        {
            for (int i = 0; i < nGramaArray.Count; i++)
            {
                if (lastComands == nGramaArray[i].GetCombination())
                {
                    //Aquí se recogen las probs de las cadenas de texto.
                    //Debug.Log("He encontrado la cadena de texto y su posicion es " + i + " y su valor es " + nGramaArray[i].GetCombination());
                    stringPosition = i;
                    return true;
                }
            }
        }

        return false;
    }

    //devuelve cual es la mejor probabilidad, siendo 0 Piedra, 1 Papel y 2 Tijeras.
    public float getBestOptionInArray(int arrayPosition)
    {
        //es 0 el mayor y unico
        if (nGramaArray[arrayPosition].GetProbability(0) > nGramaArray[arrayPosition].GetProbability(1) &&
            nGramaArray[arrayPosition].GetProbability(0) > nGramaArray[arrayPosition].GetProbability(2))
            return 0;

        //es 1 el mayor y unico
        else if (nGramaArray[arrayPosition].GetProbability(1) > nGramaArray[arrayPosition].GetProbability(0) &&
                 nGramaArray[arrayPosition].GetProbability(1) > nGramaArray[arrayPosition].GetProbability(2))
            return 1;

        //es 2 el mayor y unico
        else if (nGramaArray[arrayPosition].GetProbability(2) > nGramaArray[arrayPosition].GetProbability(0) &&
                 nGramaArray[arrayPosition].GetProbability(2) > nGramaArray[arrayPosition].GetProbability(1))
            return 2;

        //son los 3 iguales
        else if (nGramaArray[arrayPosition].GetProbability(0) == nGramaArray[arrayPosition].GetProbability(1) &&
            nGramaArray[arrayPosition].GetProbability(0) == nGramaArray[arrayPosition].GetProbability(1) &&
            nGramaArray[arrayPosition].GetProbability(0) == nGramaArray[arrayPosition].GetProbability(1))
        {
            return Rand(0, 3);
        }
        // 0 y 1 son iguales y mayores
        else if (nGramaArray[arrayPosition].GetProbability(0) == nGramaArray[arrayPosition].GetProbability(1) && 
                 nGramaArray[arrayPosition].GetProbability(1) > nGramaArray[arrayPosition].GetProbability(2))
        {
            return Rand(0, 2);
        }
        // 0 y 2 son iguales y mayores
        else if (nGramaArray[arrayPosition].GetProbability(0) == nGramaArray[arrayPosition].GetProbability(2) && 
                 nGramaArray[arrayPosition].GetProbability(0) > nGramaArray[arrayPosition].GetProbability(1))
        {
            int i = Rand(0,2);
            if (i == 1)
                i = 2;
            return i;
        }
        // 1 y 2 son iguales y mayores
        else if (nGramaArray[arrayPosition].GetProbability(1) == nGramaArray[arrayPosition].GetProbability(2) &&
                 nGramaArray[arrayPosition].GetProbability(1) > nGramaArray[arrayPosition].GetProbability(0))
        {
            return Rand(1, 3);
        }
        //falla algo?
        return -1;

    }

    //Funcion aleatoria entre un rango establecido
    public int Rand(int min, int max)
    {
        int rand = Random.Range(min, max);
        return rand;

    }

    //funcion que traduce el posible movimiento del jugador por su contrario ganador
    public string WinningMove(string playerMove)
    {
        if (playerMove == "R")
            return "P";
        else if (playerMove == "P")
            return "S";
        else if (playerMove == "S")
            return "R";
        return "fallado";
    }

    //funcion que convierte de 0, 1, y 2 a Rock, Paper y Scissors
    public string ConvertToChoice(float choice)
    {
        if (choice == 0)
            return "R";
        else if (choice == 1)
            return "P";
        else
            return "S";        
    }

    //Seleccion de piedra papel o tijera aleatoria
    public void SelectRand()
    {
        int rand = Random.Range(0, 3);

        if (rand == 0)
        {
            gc.setPlayer2("R");
            gc.p2Text.text = "R";
        }
        else if (rand == 1)
        {
            gc.setPlayer2("P");
            gc.p2Text.text = "P";
        }

        else if (rand == 2)
        {
            gc.setPlayer2("S");
            gc.p2Text.text = "S";
        }
    }
}
