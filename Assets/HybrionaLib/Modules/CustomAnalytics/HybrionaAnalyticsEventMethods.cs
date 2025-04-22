/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  HybrionaAnalyticsEventMethods.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  21-10-2023 13:06:47

*************************************************************************/
using UnityEngine;

namespace Hybriona
{
	public sealed partial class HybrionaAnalytics : MonoBehaviour
	{
        public static void ReportOnLevelStarted(int levelNum)
        {
            ReportCustomEvent("levelStartedInt", JsonUtility.ToJson( new IntegerVal() { value = levelNum }));
            
        }

        public static void ReportOnLevelCompleted(int levelNum)
        {
            ReportCustomEvent("levelCompletedInt", JsonUtility.ToJson(new IntegerVal() { value = levelNum }));
        }

        public static void ReportOnLevelStarted(string levelName)
        {
            ReportCustomEvent("levelStartedStr", JsonUtility.ToJson(new StringVal() { value = levelName }));
        }

        public static void ReportOnLevelCompleted(string levelName)
        {
            ReportCustomEvent("levelCompletedStr", JsonUtility.ToJson(new StringVal() { value = levelName }));

        }

        public static void ReportKeyValue(string key,string value)
        {
            ReportCustomEvent("evtKeyVal", JsonUtility.ToJson(new StringKeyVal() {key = key, value = value }));
        }

        public static void ReportAdRequest(string unitId)
        {
            ReportCustomEvent("adRequest", JsonUtility.ToJson(new StringVal() { value = unitId }));
        }

        public static void ReportAdImpression(string unitId)
        {
            ReportCustomEvent("adImpression", JsonUtility.ToJson(new StringVal() { value = unitId }));
        }

    }
}
