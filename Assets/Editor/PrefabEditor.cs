using JetBrains.Annotations;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class PrefabEditor : MonoBehaviour
{
    // ������ ��ġ�� ��� ������ �ν��Ͻ��� ������Ʈ�� ����
    [MenuItem("Tools/Change Animator Controllers")]
    public static void ChangeAnimatorControllers()
    {
        // ������ AnimatorController�� Resources �������� �ε�
        // AnimatorController newController = Resources.Load<AnimatorController>("CityFemaleAnimator");
        AnimatorController newController = Resources.Load<AnimatorController>("CityMaleAnimator");

        if (newController == null)
        {
            Debug.LogError("AnimatorController�� ã�� �� �����ϴ�. ��θ� Ȯ���ϼ���.");
            return;
        }

        // ���� �ִ� ��� GameObject ã��
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            // Animator ������Ʈ ��������
            Animator animator = obj.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                // AnimatorController ����
                animator.runtimeAnimatorController = newController;
                Debug.Log($"Changed {obj.name}'s AnimatorController to {newController.name}.");
            }
        }
    }
}
