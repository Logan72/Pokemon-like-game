using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_02
{
    internal static class Utility
    {
        static void GetLTRBRowColumn(int row, int column, int dRow, int dColumn, out int[][] rc)
        {
            int[][] arr = new int[4][];
            int[] left = { row, column - 1 };
            int[] top = { row - 1, column };
            int[] right = { row, column + 1 };
            int[] bottom = { row + 1, column };
            rc = arr;

            if (row <= dRow && column < dColumn)
            {
                arr[0] = right;
                arr[1] = bottom;
                arr[2] = left;
                arr[3] = top;
            }
            if (row < dRow && column >= dColumn)
            {
                arr[0] = bottom;
                arr[1] = left;
                arr[2] = top;
                arr[3] = right;
            }
            if (row > dRow && column <= dColumn)
            {
                arr[0] = top;
                arr[1] = right;
                arr[2] = bottom;
                arr[3] = left;
            }
            if (row >= dRow && column > dColumn)
            {
                arr[0] = left;
                arr[1] = top;
                arr[2] = right;
                arr[3] = bottom;
            }
        }

        static bool IsPathFoundInside(bool[,] array, ref bool onEdge, int row, int column, int dRow, int dColumn)
        {
            GetLTRBRowColumn(row, column, dRow, dColumn, out int[][] rc);

            for (int i = 0; i < rc.Length; i++)
            {
                if (!(rc[i][0] < 0 || rc[i][0] > (array.GetLength(0) - 1) || rc[i][1] < 0 || rc[i][1] > (array.GetLength(1) - 1)))
                {
                    if (rc[i][0] == dRow && rc[i][1] == dColumn) return true;

                    if (!array[rc[i][0], rc[i][1]])
                    {
                        if (!(rc[i][0] == 0 || rc[i][0] == (array.GetLength(0) - 1) || rc[i][1] == 0 || rc[i][1] == (array.GetLength(1) - 1)))
                        {
                            array[rc[i][0], rc[i][1]] = true;

                            if (IsPathFoundInside(array, ref onEdge, rc[i][0], rc[i][1], dRow, dColumn)) return true;
                        }
                        else if (!onEdge) onEdge = true;
                    }
                }
                else if (!onEdge) onEdge = true;
            }
                        
            return false;
        }

        internal static bool IsReachable(bool[,] array, int row, int column, int dRow, int dColumn)
        {
            bool onEdge1 = false;
            bool onEdge2 = false;

            bool b = IsPathFoundInside(array, ref onEdge1, row, column, dRow, dColumn);

            if (b) return true;
            else if(onEdge1)
            {
                IsPathFoundInside(array, ref onEdge2, dRow, dColumn, row, column);

                if (onEdge2) return true;
                else return false;
            }
            else return false;
        }
    }
}
