// using System.Collections.Generic;
// using Code.Scenes.BattleScene.Experimental;
// using Entitas;
// using Entitas.Unity;
// using TMPro;
// using UnityEngine;
//
// namespace Code.BattleScene.ECS.Systems
// {
//     public class UpdateOutlineMaterialsSystem : ReactiveSystem<GameEntity>
//     {
//         private readonly GameContext gameContext;
//         private readonly List<Material> materials;
//
//         public UpdateOutlineMaterialsSystem(Contexts contexts, int totalTeamsCount) : base(contexts.game)
//         {
//             gameContext = contexts.game;
//             materials = TeamsColorManager.Instance().GetOutlineMaterials(totalTeamsCount);
//         }
//
//         protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
//         {
//             return context.CreateCollector(GameMatcher.Team);
//         }
//
//         protected override bool Filter(GameEntity entity)
//         {
//             return entity.hasView && entity.hasTeam;
//         }
//
//         protected override void Execute(List<GameEntity> entities)
//         {
//             foreach (var entity in entities)
//             {
//                 var teamMaterial = materials[entity.team.id];
//                 var transform = entity.view.gameObject.transform;
//                 var renderer = transform.GetComponent<Renderer>();
//                 if (renderer != null) renderer.material = teamMaterial;
//
//                 foreach (Transform childTrans in transform)
//                 {
//                     if (childTrans.gameObject.GetEntityLink() != null) continue;
//                     var textMeshPro = childTrans.GetComponent<TextMeshPro>();
//                     if (textMeshPro != null) continue;
//                     var particleSystem = childTrans.GetComponent<ParticleSystem>();
//                     if (particleSystem != null) continue;
//                     var lineRenderer = childTrans.GetComponent<LineRenderer>();
//                     if (lineRenderer != null) continue;
//
//                     renderer = childTrans.GetComponent<Renderer>();
//                     if (renderer != null) renderer.material = teamMaterial;
//                 }
//             }
//         }
//     }
// }