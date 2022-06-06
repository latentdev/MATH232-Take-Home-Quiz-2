using Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Math.WordOperationTable;

namespace TakeHomeQuiz2Project
{
    internal class TestRig
    {
        internal void TestingGround()
        {
            var matrix = new Matrix(new double[,] { { 1, 0 }
                                                    });

            var matrix2 = new Matrix(new double[,] { { 1, 1 },
                                                     { 1, 1 }
                                                   });

            matrix.WriteLine();
            Console.WriteLine("*");
            matrix2.WriteLine();
            Console.WriteLine("=");
            var product = matrix.Multiply(matrix2);
            product.ToBinaryMatrix().WriteLine();
            Console.WriteLine();
            Console.Write("Enter a bit depth: ");
            var bitDepth = Convert.ToInt32(Console.ReadLine());
            var words = Words.GetWords(bitDepth).ToMatrices();
            var distances = new int[words.Count];
            for (int i = 0; i < words.Count; i++)
            {
                distances[i] = words[i].GetMinDistance();
                words[i].WriteLine($" Distance: {distances[i]}");
            }
            Console.WriteLine($"Min Distance: {distances.Max()}");
            Console.WriteLine();
            WordOperationTable.Write(words, words, Operation.AND);
            Console.WriteLine();


            Console.WriteLine();
            var orResult = words[0].OR(words[1]);
            words[0].WriteLine();
            Console.WriteLine("OR");
            words[1].WriteLine();
            Console.WriteLine("=");
            orResult.WriteLine();
            Console.ReadLine();
        }
    }
}
