using UnityEngine;

public interface ITaskHandler
{
    void StartTask(GameObject agent);
    void StopTask();
}