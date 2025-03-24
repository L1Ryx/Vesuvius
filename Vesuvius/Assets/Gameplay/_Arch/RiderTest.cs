using System;
using UnityEngine;

public class RiderTest : MonoBehaviour
{
    [SerializeField] int number;
    void Start()
    {
        PrintNumbersInRangeInclusive(3, 7);
    }

    /// <summary>
    /// Prints all numbers inclusively within the specified range, starting from numA to numB.
    /// </summary>
    /// <param name="numA">The starting number of the range.</param>
    /// <param name="numB">The ending number of the range.</param>
    void PrintNumbersInRangeInclusive(int numA, int numB) 
    {
        for (int i = numA; i <= numB; i++)
        {
            Debug.Log(i);
        }
    }

    /// <summary>
    /// Multiplies two float values and returns the result.
    /// </summary>
    /// <param name="a">The first float value to multiply.</param>
    /// <param name="b">The second float value to multiply.</param>
    /// <returns>The product of the two float values.</returns>
    float MultiplyTwoValues(float a, float b)
    {
        return a * b;
    }
}
