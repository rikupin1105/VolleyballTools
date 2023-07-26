namespace VolleyballTools.Model
{
    public class Match : Competition
    {
        /// <summary>
        /// 性別
        /// </summary>
        public Sex? Sex { get; set; }

        /// <summary>
        /// 試合番号
        /// </summary>
        public string? MatchNumber { get; set; }
    }
}
