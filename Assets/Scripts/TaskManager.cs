using UnityEngine;

public class TaskManager : MonoBehaviour
{
    private ITaskHandler currentTask;

    private void OnTriggerEnter(Collider other)
    {
        // 부딪힌 오브젝트에서 ITaskZone 인터페이스를 구현한 컴포넌트를 찾음
        ITaskHandler task = other.GetComponent<ITaskHandler>();

        if (task != null)
        {
            currentTask = task;
            currentTask.StartTask(gameObject);
            Debug.Log($"{other.name} 작업 시작");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ITaskHandler task = other.GetComponent<ITaskHandler>();

        // 현재 작업 중인 구역에서 나가는 경우에만 정지
        if (task != null && task == currentTask)
        {
            currentTask.StopTask();
            currentTask = null;
            Debug.Log($"{other.name} 작업 종료");
        }
    }
}