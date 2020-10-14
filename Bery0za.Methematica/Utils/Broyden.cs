using System;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Properties;

using static Bery0za.Methematica.Helpers;
#if DOUBLE
using Real = System.Double;
using Math = System.Math;
using MathNet.Numerics.LinearAlgebra.Double;
#else
using Real = System.Single;
using Math = Bery0za.Methematica.MathF;

using MathNet.Numerics.LinearAlgebra.Single;

#endif

namespace Bery0za.Methematica.Utils
{
    public static class Broyden
    {
        public static Real[] FindRoot(Func<Real[], Real[]> f,
                                      Real[] initialGuess,
                                      Real accuracy = Numerics.EPSILON,
                                      int maxIterations = 100,
                                      Real jacobianStepSize = Numerics.JACOBIAN_STEP)
        {
            Real[] root;

            if (TryFindRootWithJacobianStep(f, initialGuess, accuracy, maxIterations, jacobianStepSize, out root))
                return root;

            throw new NonConvergenceException(Resources.RootFindingFailed);
        }

        public static bool TryFindRootWithJacobianStep(Func<Real[], Real[]> f,
                                                       Real[] initialGuess,
                                                       Real accuracy,
                                                       int maxIterations,
                                                       Real jacobianStepSize,
                                                       out Real[] root)
        {
            DenseVector denseVector1 = new DenseVector(initialGuess);
            Real[] numArray = f(initialGuess);
            DenseVector denseVector2 = new DenseVector(numArray);
            Real num1 = ToReal(denseVector2.L2Norm());

            Matrix<Real> approximateJacobian =
                Broyden.CalculateApproximateJacobian(f, initialGuess, numArray, jacobianStepSize);

            for (int index = 0; index <= maxIterations; ++index)
            {
                DenseVector denseVector3 = (DenseVector)(-approximateJacobian.LU().Solve(denseVector2));
                DenseVector denseVector4 = denseVector1 + denseVector3;
                DenseVector denseVector5 = new DenseVector(f(denseVector4.Values));
                Real num2 = ToReal(denseVector5.L2Norm());

                if (num2 > num1)
                {
                    Real num3 = num1 * num1;
                    Real num4 = num3 / (num3 + num2 * num2);
                    if (num4 == 0.0) num4 = ToReal(0.0001);
                    denseVector3 = num4 * denseVector3;
                    denseVector4 = denseVector1 + denseVector3;
                    denseVector5 = new DenseVector(f(denseVector4.Values));
                    num2 = ToReal(denseVector5.L2Norm());
                }

                if (num2 < accuracy)
                {
                    root = denseVector4.Values;

                    return true;
                }

                Matrix<Real> matrix =
                    (denseVector5 - denseVector2 - approximateJacobian.Multiply(denseVector3)).ToColumnMatrix()
                    * denseVector3.Multiply(ToReal(1.0) / Math.Pow(ToReal(denseVector3.L2Norm()), ToReal(2.0)))
                                  .ToRowMatrix();

                approximateJacobian += matrix;
                denseVector1 = denseVector4;
                denseVector2 = denseVector5;
                num1 = num2;
            }

            root = null;

            return false;
        }

        /// <summary>Find a solution of the equation f(x)=0.</summary>
        /// <param name="f">The function to find roots from.</param>
        /// <param name="initialGuess">Initial guess of the root.</param>
        /// <param name="accuracy">Desired accuracy. The root will be refined until the accuracy or the maximum number of iterations is reached.</param>
        /// <param name="maxIterations">Maximum number of iterations. Usually 100.</param>
        /// <param name="root">The root that was found, if any. Undefined if the function returns false.</param>
        /// <returns>True if a root with the specified accuracy was found, else false.</returns>
        public static bool TryFindRoot(Func<Real[], Real[]> f,
                                       Real[] initialGuess,
                                       Real accuracy,
                                       int maxIterations,
                                       out Real[] root)
        {
            return Broyden.TryFindRootWithJacobianStep(f,
                                                       initialGuess,
                                                       accuracy,
                                                       maxIterations,
                                                       ToReal(0.0001),
                                                       out root);
        }

        /// <summary>
        /// Helper method to calculate an approximation of the Jacobian.
        /// </summary>
        /// <param name="f">The function.</param>
        /// <param name="x0">The argument (initial guess).</param>
        /// <param name="y0">The result (of initial guess).</param>
        /// <param name="jacobianStepSize">Relative step size for calculating the Jacobian.</param>
        private static Matrix<Real> CalculateApproximateJacobian(Func<Real[], Real[]> f,
                                                                 Real[] x0,
                                                                 Real[] y0,
                                                                 Real jacobianStepSize)
        {
            int length = x0.Length;
            DenseMatrix denseMatrix = new DenseMatrix(length);
            Real[] numArray1 = new Real[length];
            Array.Copy(x0, 0, numArray1, 0, length);

            for (int column = 0; column < length; ++column)
            {
                Real num1 = ToReal(1.0 + Math.Abs(x0[column])) * jacobianStepSize;
                Real num2 = numArray1[column];
                numArray1[column] = num2 + num1;
                Real[] numArray2 = f(numArray1);
                numArray1[column] = num2;
                for (int row = 0; row < length; ++row) denseMatrix.At(row, column, (numArray2[row] - y0[row]) / num1);
            }

            return denseMatrix;
        }
    }
}