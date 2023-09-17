using GoogleMobileAds.Api;
using System.Collections.Generic;

namespace Hybriona
{
    public enum AdSource { AdMob = 0, InHouse = 1 };
    public enum AdType { Interstitial = 0, Rewarded = 1, Banner = 2 };
    public class AdsManager
    {
        private static List<AdUnit> adunits = new List<AdUnit>();
        public static void Initialize(System.Action completion)
        {
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            MobileAds.Initialize((initStatus) =>
            {
                if (completion != null)
                {
                    completion();
                }
            });
        }


        public static void CreateAdUnit(AdUnit adunit)
        {
            if (!adunits.Contains(adunit))
            {
                adunits.Add(adunit);
            }
        }

        public static void RemoveAdUnit(AdUnit adunit)
        {
            if (adunits.Contains(adunit))
            {
                adunits.Remove(adunit);
                adunit.Dispose();
                adunit = null;
            }
        }
    }




}
