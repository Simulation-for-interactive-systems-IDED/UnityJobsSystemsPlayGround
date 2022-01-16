using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TestingJobIsComplete : MonoBehaviour
{
    private struct DoSomeJob : IJob
    {
        [ReadOnly]
        public uint MaxNum;
        public NativeArray<uint> Result;

        public void Execute()
        {
            // Do something
            uint sum = 0;
            for (uint i = 1; i <= MaxNum; i++)
            {
                sum += i;
            }
            Result[0] = sum;

            // Add a delay so the job takes some time to complete
            // Thread.Sleep(16 * 28);
        }
    }

    NativeArray<uint> m_myJobResult;
    JobHandle m_myJobHandle;
    private bool m_alreadyCompleted = false;

    // Start is called before the first frame update
    void Start()
    {
        m_myJobResult = new NativeArray<uint>(1, Allocator.Persistent);
        var myJob = new DoSomeJob()
        {
            MaxNum = 2,
            Result = m_myJobResult
        };

        m_myJobHandle = myJob.Schedule();

        // JobHandle.ScheduleBatchedJobs();
        Debug.Log($"<color=yellow>Started on frame: {Time.frameCount}</color>");
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_alreadyCompleted && m_myJobHandle.IsCompleted)
        {
            // NOTE: This is required to avoid an exception stating "... You must call JobHandle.Complete() on the job TestingJobIsComplete:DoSomeJob, before you can read from the Unity.Collections.NativeArray`1[System.UInt32] safely"
            // Tracing data ownership requires dependencies
            // to complete before the main thread can use them again. It is not enough to check JobHandle.IsCompleted
            // https://docs.unity3d.com/Manual/JobSystemTroubleshooting.html
            m_myJobHandle.Complete();
            Debug.Log($"<color=green>Completed with result {m_myJobResult[0]}</color>");
            m_alreadyCompleted = true;
        } 
        else if (!m_alreadyCompleted)
        {
            Debug.Log($"<color=yellow>Waiting during frame {Time.frameCount}</color>");
        }
    }

    private void OnDisable()
    {
        m_myJobResult.Dispose();
    }
}
