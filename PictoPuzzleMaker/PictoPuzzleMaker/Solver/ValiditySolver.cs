using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PictoPuzzleMaker.Solver
{
    public static class ValiditySolver
    {

        public static int[,] GetSolvedGrid(int size, int[][] horizontalClues, int[][] verticalClues)
        {

            //we could make this without the original grid, but having access to this solved grid might make it easier to 
            //show where unsolvable tiles are

            //won't be too slow or anything, either

            //instantiate new grid and clear it
            int[,] grid = new int[size, size];
            for (int y = 0; y < size; y++)
            {
                for(int x = 0; x < size; x++)   
                {
                    grid[y, x] = 0;
                }
            }

            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int x = 0; x < size; x++)
                {
                    int[] col = new int[size];
                    for(int y  = 0; y < size; y++)
                    {
                        col[y] = grid[y, x];
                    }
                    col = solveLine(verticalClues[x], col);
                    for (int y = 0; y < size; y++)
                    {
                        if(col[y] != 0 && col[y] != grid[y,x])
                            changed = true;
                        grid[y, x] = col[y];
                    }
                }

                for(int y = 0;y < size; y++)
                {
                    int[] row = new int[size];
                    for(int x = 0;x < size; x++)
                    {
                        row[x] = grid[y, x];
                    }
                    row = solveLine(horizontalClues[y], row);
                    for(int x = 0; x < size; x++)
                    {
                        if(row[x] != 0 && row[x] != grid[y, x])
                            changed = true; 
                        grid[y, x] = row[x];
                    }
                }
            }
            return grid;
           
        }

        static private int[] solveLine (int[] clues, int[] line)
        {
            int[][] permutations = getPermutations(clues, line.Length);
            List<int[]> validPermutations = new List<int[]>();

            foreach (int[] permutation in permutations)
            {
                bool valid = true;
                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] != 0 && line[x] != permutation[x])
                        valid = false;
                }
                if (valid)
                    validPermutations.Add(permutation);
            }

            int[] newLine = validPermutations[0].Slice();

            foreach (int[] permutation in validPermutations)
            {
                for(int x = 0; x < line.Length; x++)
                {
                    if (newLine[x] != permutation[x])
                        newLine[x] = 0;
                }
            }

            return newLine;
        }

        static private int[][] getPermutations(int[] clues, int length)
        {
            if(clues.Length == 0)
            {
                int[] row = new int [length];
                for(int i = 0; i < length; i++)
                    row[i] = 2;
                return new int[][] {row};
            }

            List<int[]> permutations = new List<int[]>();

            for(int i = 0;i < length - clues[0] + 1; i++)
            {
                int[] permutation = new int[length];

                int x;
                for(x = 0; x < i; x++)
                {
                    permutation[x] = 2;
                }
                for(x = i; x < i + clues[0]; x++)
                {
                    permutation[x] = 1;
                }
                x = i + clues[0];

                if(x < length)
                {
                    permutation[x] = 2;
                    x += 1;
                }

                if(x == length && clues.Length == 0)
                {
                    permutations.Add(permutation);
                    break;
                }

                int[][] subRows = getPermutations(clues.Slice(1, clues.Length), length - x);

                foreach (int[] subRow in subRows)
                {
                    int[] subPermutation = permutation.Slice();

                    for(int k = x; k < length; k++)
                    {
                        subPermutation[k] = subRow[k - x];
                    }
                    permutations.Add(subPermutation);
                }
                
            }

            return permutations.ToArray();

        }


    }

    //found in this stack overflow thread:
    //https://stackoverflow.com/questions/1792470/subset-of-array-in-c-sharp
    static class ArrayUtilities
    {
        // create a subset from a range of indices
        public static T[] RangeSubset<T>(this T[] array, int startIndex, int length)
        {
            T[] subset = new T[length];
            Array.Copy(array, startIndex, subset, 0, length);
            return subset;
        }

        // create a subset from a specific list of indices
        public static T[] Subset<T>(this T[] array, params int[] indices)
        {
            T[] subset = new T[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                subset[i] = array[indices[i]];
            }
            return subset;
        }

        //Create a subset from start and end indicies
        public static T[] Slice<T>(this T[] array, int startIndex = -1, int endIndex = -1)
        {
            if(startIndex < 0)
            {
                return RangeSubset(array, 0, array.Length);

            }

            if(endIndex < 0)
            {
                endIndex = array.Length;
            }
            int length = endIndex - startIndex;
            
            return RangeSubset(array, startIndex, length);

        }
    }
}
