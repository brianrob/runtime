// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#include "common.h"

static Volatile<bool> s_isYieldProcessorNormalizedInitialized = false;
static CrstStatic s_initializeYieldProcessorNormalizedCrst;

void InitializeYieldProcessorNormalizedCrst()
{
    WRAPPER_NO_CONTRACT;
    s_initializeYieldProcessorNormalizedCrst.Init(CrstLeafLock);
}

static void InitializeYieldProcessorNormalized()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        MODE_PREEMPTIVE;
    }
    CONTRACTL_END;

    if (s_isYieldProcessorNormalizedInitialized)
    {
        return;
    }


    DWORD yieldsPerNormalizedYield = CLRConfig::GetConfigValue(CLRConfig::INTERNAL_Thread_YieldsPerNormalizedYield);
    DWORD optimalMaxNormalizedYieldsPerSpinIteration = CLRConfig::GetConfigValue(CLRConfig::INTERNAL_Thread_YieldsPerSpinIteration);

    g_yieldsPerNormalizedYield = yieldsPerNormalizedYield;
    g_optimalMaxNormalizedYieldsPerSpinIteration = optimalMaxNormalizedYieldsPerSpinIteration;
    s_isYieldProcessorNormalizedInitialized = true;

    GCHeapUtilities::GetGCHeap()->SetYieldProcessorScalingFactor((float)yieldsPerNormalizedYield);
    printf("YieldsPerNormalizedYield: %d\n", yieldsPerNormalizedYield);
    printf("OptimalMaxNormalizedYieldsPerSpinIteration: %d\n", optimalMaxNormalizedYieldsPerSpinIteration);
}

void EnsureYieldProcessorNormalizedInitialized()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        MODE_PREEMPTIVE;
    }
    CONTRACTL_END;

    if (!s_isYieldProcessorNormalizedInitialized)
    {
        InitializeYieldProcessorNormalized();
    }
}
