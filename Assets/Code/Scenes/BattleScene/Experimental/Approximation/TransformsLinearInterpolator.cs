// using System.Collections.Generic;
// using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
//
// namespace Code.Scenes.BattleScene.Experimental.Approximation
// {
//     public class TransformsLinearInterpolator : AbstractLinearInterpolator<ViewTransform>
//     {
//         private readonly Dictionary<ushort, ViewTransform> oldTransformsToCorrect;
//
//         public TransformsLinearInterpolator(float time) : base(time)
//         {
//             oldTransformsToCorrect = new Dictionary<ushort, ViewTransform>();
//         }
//
//         public override void Set(Dictionary<ushort, ViewTransform> transforms, float time)
//         {
//             base.Set(transforms, time);
//             // Костыли, чтобы не было поворотов >180
//             foreach (var old in oldValues)
//             {
//                 var oldKey = old.Key;
//                 var oldVal = old.Value;
//
//                 if (oldVal.Angle < 0f)
//                 {
//                     oldVal.Angle += 360f;
//                     oldTransformsToCorrect.Add(oldKey, oldVal);
//                 }
//                 else if (oldVal.Angle >= 360f)
//                 {
//                     oldVal.Angle -= 360f;
//                     oldTransformsToCorrect.Add(oldKey, oldVal);
//                 }
//
//                 if (newValues.TryGetValue(oldKey, out var newTransform))
//                 {
//                     if (newTransform.Angle - oldVal.Angle > 180f)
//                     {
//                         newTransform.Angle -= 360f;
//                         newValues[oldKey] = newTransform;
//                     }
//                     else if(newTransform.Angle - oldVal.Angle < -180f)
//                     {
//                         newTransform.Angle += 360f;
//                         newValues[oldKey] = newTransform;
//                     }
//                 }
//             }
//
//             foreach (var pair in oldTransformsToCorrect)
//             {
//                 oldValues[pair.Key] = pair.Value;
//             }
//
//             oldTransformsToCorrect.Clear();
//         }
//
//         protected override ViewTransform GetValue(ViewTransform oldTransform, ViewTransform newTransform, float percent)
//         {
//             return oldTransform + (newTransform - oldTransform) * percent;
//         }
//     }
// }
