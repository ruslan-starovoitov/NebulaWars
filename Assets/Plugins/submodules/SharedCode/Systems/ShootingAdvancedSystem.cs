// using SharedSimulationCode.LagCompensation;
//
// namespace SharedSimulationCode
// {
//     public class ShootingAdvancedSystem
//     {
//         public void Execute(GameState gs)
//         {
//             foreach (var shotPair in gs.WorldState.Shot)
//             {
//                 var shot = shotPair.Value;
//                 var shooter = gs.WorldState[shotPair.Key];
//                 var shooterTransform = shooter.Transform;
//                 var weaponStats = gs.WorldState.WeaponStats[shot.WeaponId];
//
//                 // DeltaTime shouldn't exceed physics history size
//                 var shootDeltaTime = (int) (gs.Time - shot.ShotPlayerWorldTime);
//                 if (shootDeltaTime > PhysicsWorld.HistoryLength)
//                 {
//                     continue;
//                 }
//
//                 // Get the world at the time of shooting.
//                 var oldState = _immutableHistory.Get(shot.ShotPlayerWorldTime);
//         
//                 var potentialTarget = oldState.WorldState[shot.Target.Id];
//                 var hitTargetId = _singleShotValidator.ValidateTargetAvailabilityInLine(oldState, potentialTarget, shooter,
//                     shootDeltaTime, weaponStats.ShotDistance, shooter.Transform.Angle.GetDirection());
//
//                 if (hitTargetId != 0)
//                 {    
//                     gs.WorldState.CreateEntity().AddDamage(gs.WorldState[hitTargetId], shooter, weaponStats.ShotDamage);
//                 }
//             }
//         }
//     }
// }