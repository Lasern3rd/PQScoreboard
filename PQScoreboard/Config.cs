using System;
using System.Linq;

namespace PQScoreboard
{
    public class Config
    {
        public static Config Values { get; set; }

        public string FontTeamNames { get; set; } = "Yu Gothic UI Light";

        public string FontCategoryNames { get; set; } = "Yu Gothic UI Light";

        public double FireworksRadiusMin { get; set; } = 75d;

        public double FireworksRadiusScale { get; set; } = 100d;

        public void Validate()
        {
            // backbuffer is 1920 x 1080
            // left and right margin is 100 => 2 * (FireworksRadiusMin + FireworksRadiusScale) must be less than 1720
            // top margin is 10 and bottom margin is 150 => 2 * (FireworksRadiusMin + FireworksRadiusScale) must be less than 920
            // and we also want some variation in both dimensions, hence these values seem reasonable
            if (FireworksRadiusMin < 50d || FireworksRadiusMin > 150d)
            {
                throw new ArgumentException("Invalid value in config: 'FireworksRadiusMin' must be in [50, 200]");
            }
            if (FireworksRadiusScale < 50d || FireworksRadiusScale > 200d)
            {
                throw new ArgumentException("Invalid value in config: 'FireworksRadiusMin' must be in [50, 300]");
            }
        }
    }
}
