using JetBrains.Annotations;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class PrefabEditor : MonoBehaviour
{
    // 레벨에 배치된 모든 프리팹 인스턴스의 컴포넌트를 수정
    [MenuItem("Tools/Change Animator Controllers")]
    public static void ChangeAnimatorControllers()
    {
        // 변경할 AnimatorController를 Resources 폴더에서 로드
        // AnimatorController newController = Resources.Load<AnimatorController>("CityFemaleAnimator");
        AnimatorController newController = Resources.Load<AnimatorController>("TEST");

        if (newController == null)
        {
            Debug.LogError("AnimatorController를 찾을 수 없습니다. 경로를 확인하세요.");
            return;
        }

        // 씬에 있는 모든 GameObject 찾기
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            // Animator 컴포넌트 가져오기
            Animator animator = obj.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                // AnimatorController 변경
                animator.runtimeAnimatorController = newController;
                Debug.Log($"Changed {obj.name}'s AnimatorController to {newController.name}.");
            }
        }
    }

    [MenuItem("Tools/Change Rigidbody Constraints")]
    public static void ChangeRigidbodyContraints()
    {
        // 씬에 있는 모든 GameObject 찾기
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            Rigidbody rigidbody = obj.GetComponentInChildren<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            }
        }
    }
}
