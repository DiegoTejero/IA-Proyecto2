using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class N_Grama {

    private string combination;
    public float[] probability = new float[4];

    public N_Grama (string cadena, float[] probabilities)
    {
        SetCombination(cadena);
        SetProbability(probabilities);
	}

    public string GetCombination()
    {
        return combination;
    }

    public void SetCombination(string str)
    {
        combination = str;
    }

    public float GetProbability(int p)
    {
        return probability[p];
    }

    public float[] GetProbabilities()
    {
        return probability;
    }

    public void SetProbability(float[] prob)
    {
        probability[0] += prob[0];
        probability[1] += prob[1];
        probability[2] += prob[2];
        probability[3] = probability[0] + probability[1] + probability[2];
    }

    public float getTotal()
    {
        return probability[3];
    }

}
