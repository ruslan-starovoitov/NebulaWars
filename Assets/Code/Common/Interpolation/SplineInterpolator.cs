// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// namespace Code.Common.Interpolation
// {
//     /// <summary>
//     /// Spline interpolation class.
//     /// </summary>
//     public class SplineInterpolator
//     {
//         private readonly double[] a;
//         private readonly double[] h;
//         private readonly double[] xs;
//         private readonly double[] ys;
//         
//         public SplineInterpolator(IDictionary<double, double> nodes)
//         {
//             if (nodes == null)
//             {
//                 throw new ArgumentNullException("nodes");
//             }
//
//             int n = nodes.Count;
//
//             if (n < 2)
//             {
//                 throw new ArgumentException("At least two point required for interpolation.");
//             }
//
//             xs = nodes.Keys.ToArray();
//             ys = nodes.Values.ToArray();
//             a = new double[n];
//             h = new double[n];
//
//             for (int i = 1; i < n; i++)
//             {
//                 h[i] = xs[i] - xs[i - 1];
//             }
//
//             if (n > 2)
//             {
//                 double[] sub = new double[n - 1];
//                 double[] diag = new double[n - 1];
//                 double[] sup = new double[n - 1];
//
//                 for (int i = 1; i <= n - 2; i++)
//                 {
//                     diag[i] = (h[i] + h[i + 1]) / 3;
//                     sup[i] = h[i + 1] / 6;
//                     sub[i] = h[i] / 6;
//                     a[i] = (ys[i + 1] - ys[i]) / h[i + 1] - (ys[i] - ys[i - 1]) / h[i];
//                 }
//
//                 SolveTridiag(sub, diag, sup, ref a, n - 2);
//             }
//         }
//         
//         public Vector2 GetValue(float progress)
//         {
//             double xMinValue = xs[2];
//             double xMaxValue = xs[3];
//             
//             int nextIndex = 3;
//             double xValue = xMinValue + progress*(xMaxValue-xMinValue);
//
//             double x1 = xValue - xMinValue;
//             double xDelta = h[nextIndex] - x1;
//             double y =  ((-a[nextIndex - 1] / 6 * (xDelta + h[nextIndex]) * x1 + ys[nextIndex - 1]) * xDelta +
//                     (-a[nextIndex] / 6 * (x1 + h[nextIndex]) * xDelta + ys[nextIndex]) * x1) / h[nextIndex];
//             
//             return new Vector2((float)xValue, (float)y);
//         }
//         
//         private static void SolveTridiag(double[] sub, double[] diag, double[] sup, ref double[] b, int n)
//         {
//             int i;
//
//             for (i = 2; i <= n; i++)
//             {
//                 sub[i] = sub[i] / diag[i - 1];
//                 diag[i] = diag[i] - sub[i] * sup[i - 1];
//                 b[i] = b[i] - sub[i] * b[i - 1];
//             }
//
//             b[n] = b[n] / diag[n];
//             
//             for (i = n - 1; i >= 1; i--)
//             {
//                 b[i] = (b[i] - sup[i] * b[i + 1]) / diag[i];
//             }
//         }
//     }
// }