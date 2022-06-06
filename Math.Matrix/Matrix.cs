using System.Text;

namespace Math
{
    public class Matrix
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public double[,] Values { get; set; }

        public int ConsoleWidth { get => Columns + Columns - 1 + 2; } // number of commas = Columns-1, num of Staves = 2

        public Matrix(int rows, int columns)
        {
            Columns = columns;
            Rows = rows;
            Values = new double[Rows, Columns];
        }

        public Matrix(double[,] values)
        {
            Values = values;
            Rows = values.GetLength(0);
            Columns = values.GetLength(1);
        }

        public override bool Equals(object? obj)
        {
            if(obj.GetType() != this.GetType())
                return false;
            var matrix = (Matrix)obj;
            if (matrix.Rows != this.Rows || matrix.Columns != this.Columns)
                return false;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (Values[i, j] != matrix.Values[i, j])
                        return false;
                }
            }
            return true;
        }
    }

    public static class MatrixExtensions
    {
        public static Matrix Multiply(this Matrix matrix1, Matrix matrix2)
        {

            // checking if product is defined  
            if (matrix1.Columns != matrix2.Rows)
                throw new InvalidOperationException
                  ("Product is undefined. n columns of first matrix must equal to n rows of second matrix");

            // creating the final product matrix  
            Matrix product = new Matrix(matrix1.Rows, matrix2.Columns);

            // looping through matrix 1 rows  
            for (int matrix1_row = 0; matrix1_row < matrix1.Rows; matrix1_row++)
            {
                // for each matrix 1 row, loop through matrix 2 columns  
                for (int matrix2_col = 0; matrix2_col < matrix2.Columns; matrix2_col++)
                {
                    // loop through matrix 1 columns to calculate the dot product  
                    for (int matrix1_col = 0; matrix1_col < matrix1.Columns; matrix1_col++)
                    {
                        var result = matrix1.Values[matrix1_row, matrix1_col] *
                                     matrix2.Values[matrix1_col, matrix2_col];
                        product.Values[matrix1_row, matrix2_col] += result;

                    }
                }
            }

            return product;
        }

        public static Matrix ToBinaryMatrix(this Matrix matrix)
        {
            for (int row = 0; row < matrix.Rows; row++)
            {
                for (int columns = 0; columns < matrix.Columns; columns++)
                {
                    matrix.Values[row, columns] = matrix.Values[row, columns] > 0 ? 1 : 0;
                }
            }
            return matrix;
        }

        public static string Stringify(this Matrix matrix)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < matrix.Rows; i++)
            {
                stringBuilder.Append("[");
                for (int j = 0; j < matrix.Columns; j++)
                {
                    stringBuilder.Append(matrix.Values[i, j]);
                    if (j != matrix.Columns - 1)
                    {
                        stringBuilder.Append(",");
                    }
                }
                stringBuilder.Append(i == matrix.Rows - 1 ? "]" : $"]{Environment.NewLine}");
            }
            return stringBuilder.ToString();
        }

        public static void Write(this Matrix matrix)
        {
            Console.Write(matrix.Stringify());
        }

        public static void Write(this Matrix matrix, string message)
        {
            matrix.Write();
            Console.Write(message);
        }

        public static void WriteLine(this Matrix matrix)
        {
            matrix.Write("\n");
        }

        public static void WriteLine(this Matrix matrix, string message)
        {
            matrix.Write();
            Console.WriteLine(message);
        }
    }

    public static class MatricesTable
    {

    }
}