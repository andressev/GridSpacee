using System;
using UnityEngine;

public class NxNMatrix
{
    private Cell[,] matrix;
    private int size;
    private int radius;

    // Constructor to initialize an NxN matrix
    public NxNMatrix(int n)
    {
        size = n;
        matrix = new Cell[n, n];
    }

    // Method to set a value at a specific position
    public void SetValue(int xOffset, int yOffset, Cell cell)
    {
       
        int radius= (size-1)/2; //Size always has to be an odd number

        xOffset+=radius;
        yOffset+=radius;

        if (xOffset >= 0 && xOffset < size && yOffset >= 0 && yOffset < size)
        {
            matrix[xOffset, yOffset] = cell;
        }
        else
        {
            Debug.LogError($"Out of bounds, tried to set matrix[{xOffset}, {yOffset}]" );
        }
    }

    // Method to get a value from a specific position
    public Cell GetValue(int xOffset, int yOffset)
    {

        int radius= (size-1)/2; //Size always has to be an odd number

        xOffset+=radius;
        yOffset+=radius;


        if (xOffset >= 0 && xOffset < size && yOffset >= 0 && yOffset < size)
        {
            
            return matrix[xOffset, yOffset];

        }
        else
        {
            Cell cell= null;
            Debug.LogError("xOffset and yOffsetumn must be within matrix bounds.");
            return cell;
        }
    }

    // Method to print the matrix
    public void PrintMatrix()
    {
        string matrixString = "";
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                matrixString += matrix[i, j].name + "\t";
            }
            matrixString += "\n";
        }
        Debug.Log(matrixString);
    }

    // Example of a basic matrix operation: Transpose
    public NxNMatrix Transpose()
    {
        NxNMatrix transposedMatrix = new NxNMatrix(size);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                transposedMatrix.SetValue(j, i, matrix[i, j]);
            }
        }
        return transposedMatrix;
    }

    // Example of matrix addition
    // public NxNMatrix Addi(NxNMatrix other)
    // {
    //     if (other.size != this.size)
    //     {
    //         thxOffset new ArgumentException("Matrices must be of the same size.");
    //     }

    //     NxNMatrix result = new NxNMatrix(size);

    // }

}
   
