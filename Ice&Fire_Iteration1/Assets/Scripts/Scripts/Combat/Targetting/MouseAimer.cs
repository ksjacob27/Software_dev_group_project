// using System.Collections.Generic;
// using UnityEngine;
//
//
//
// namespace Spells {
//
//     /// <summary>Aims directly at the position under the mouse cursor.
//     /// <para>Note that this gives a definite position to be aimed at as opposed to a direction in form of a forward coordinate in <c>SkillshotSpell</c></para>
//     /// </summary>
//     public class MouseAimer : Aimer {
//
//         public MouseAimer(Ability aimedSpell) : base(aimedSpell) {}
//
//         
//         /// <summary>Position of aimed ability</summary>
//         public Vector3 TargetPosition { get; private set; }
//
//         
//         /// <summary>Automated target pruning</summary>
//         public override bool DoPlayerAimAssist(float snapAngle, Vector3 aimInput, Vector3 m) {
//             float bestScore = float.MaxValue;
//         
//             // check if the player inputted directions at all
//             if (aimInput.sqrMagnitude <= 0.02f) {
//                 aimInput = ChargedAbility.p_Actor.transform.forward;
//             }
//         
//             foreach (Enemy enemy in ServerSceneManager.Enemies) {
//                 Debug.Assert(enemy != null);
//         
//                 // distance check
//                 float distanceToEnemy = Vector3.Distance( ChargedAbility.p_Actor.transform.position, enemy.transform.position);
//                 if (distanceToEnemy > ChargedAbility.p_Range) continue;
//         
//                 float angleDifference = Mathf.Abs(Vector3.SignedAngle(aimInput, enemy.transform.position -  ChargedAbility.p_Actor.transform.position, Vector3.up));
//         
//                 // evaluate scoring
//         
//                 if (angleDifference <= snapAngle && angleDifference <= bestScore) {
//                     bestScore = angleDifference;
//                     TargetPosition = enemy.transform.position;
//                 }
//             }
//             
//             // check if bestScore was set
//             if (bestScore == float.MaxValue) {
//                 return false;
//             }
//         
//             TargetRotation = Quaternion.LookRotation(TargetPosition -  ChargedAbility.p_Actor.transform.position);
//             return true;
//         }
//
//         
//         /// <summary>
//         /// 
//         /// </summary>
//         /// <param name="targets"></param>
//         /// <returns></returns>
//         public override bool DoEnemyAim(List<Player> targets) {
//             Debug.Assert(targets != null);
//             Debug.Assert(targets.Count > 0);
//
//             float   minDistanceToTarget = float.MaxValue;
//             Vector3 optimalTarget  = Vector3.zero;
//
//             foreach (Player target in targets) {
//                 float distFromTarget = Vector3.Distance(target.transform.position,  ChargedAbility.p_Actor.transform.position);
//                 
//                 if (!(distFromTarget < minDistanceToTarget)) {
//                     continue;
//                 }
//                 
//                 minDistanceToTarget = distFromTarget;
//                 optimalTarget = target.transform.position;
//             }
//
//             if (minDistanceToTarget > ChargedAbility.p_Range) { return false; }
//
//             TargetPosition = optimalTarget;
//             TargetRotation = Quaternion.LookRotation(optimalTarget - ChargedAbility.p_Actor.transform.position);
//             return true;
//         }
//     }
// }
