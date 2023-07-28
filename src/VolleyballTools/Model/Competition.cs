namespace VolleyballTools.Model
{
    public class Competition
    {
        /// <summary>
        /// 大会名
        /// </summary>
        public string? MatchName { get; set; } = null;

        /// <summary>
        /// 開催地
        /// </summary>
        public string? Venue { get; set; } = null;

        /// <summary>
        /// 会場名
        /// </summary>
        public string? Hall { get; set; } = null;

        /// <summary>
        /// 日付
        /// </summary>
        public DateTime? DateTime { get; set; } = null;

        /// <summary>
        /// 試合設定時間
        /// </summary>
        public DateTime? MatchTime { get; set; } = null;
    }
}
