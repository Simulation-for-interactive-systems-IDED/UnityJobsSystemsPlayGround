#define USE_JOBS

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

#if USE_JOBS
public struct SumNumsJob : IJob
{
    [ReadOnly]
    public int maxNumber;
    public NativeArray<int> result;

    public void Execute()
    {
        int num = 0;
        for (int i = 0; i <= maxNumber; i++)
        {
            //Thread.Sleep(250);
            num += i;
        }

        result[0] = num;
    }
}
#endif

public class HelloJob : MonoBehaviour
{
    [SerializeField]
    private int m_maxNum = 2;

#if USE_JOBS
    private NativeArray<int> m_result;
    private JobHandle m_sumNumsJobHandle;
    private bool m_alreadyFinished = false;
#endif

    // Start is called before the first frame update
    private void Start()
    {
        TestSumNums();
    }

    // Update is called once per frame
    private void Update()
    {
#if USE_JOBS
        if (!m_alreadyFinished && m_sumNumsJobHandle.IsCompleted)
        {
            //m_sumNumsJobHandle.Complete();
            print("<color=green>Completed with result " + m_result[0] + "</color>");
            m_result.Dispose();
            m_alreadyFinished = true;
        }
        else if (!m_alreadyFinished)
        {
            print("Frame " + Time.frameCount);
        }
#endif
    }

    public void TestSumNums()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        print("<color=yellow>Started TestSumNums()</color>");

#if USE_JOBS
        m_sumNumsJobHandle = ScheduleSumNums();
        print("Scheduled the SumNums jobs ");
#else
        int result = SumNums();
        print("<color=green>Completed with result " + result + "</color>");
#endif

        stopwatch.Stop();
        print("<color=yellow>Finished TestSumNums() after " + stopwatch.ElapsedMilliseconds + "ms</color>");
    }

#if !USE_JOBS
    private int SumNums()
    {
        int num = 0;
        for (int i = 0; i <= m_maxNum; i++)
        {
            Thread.Sleep(250);
            num += i;
        }

        return num;
    }
#endif

#if USE_JOBS
    private JobHandle ScheduleSumNums()
    {
        m_result = new NativeArray<int>(1, Allocator.TempJob);

        var sumNumsJob = new SumNumsJob()
        {
            maxNumber = m_maxNum,
            result = m_result
        };

        return sumNumsJob.Schedule();
    }
#endif
}
