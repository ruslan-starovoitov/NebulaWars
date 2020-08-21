// using System.Collections.Generic;
// // using Code.Common.Interpolation;
// using UnityEngine;
//
// namespace Code.Scenes.Interpolation
// {
//     public class Interolator
//     {
//         public Vector2 Do(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float progress)
//         {
//             Dictionary<double, double> points = new Dictionary<double, double>
//             { 
//                 { p0.x, p0.y },
//                 { p1.x, p1.y },
//                 { p2.x, p2.y },
//                 { p3.x, p3.y }
//             };
//
//             SplineInterpolator splineInterpolator = new SplineInterpolator(points);
//             return splineInterpolator.GetValue(progress);
//         }
//     }
// }