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

        public string? ATeamName { get; set; }
        public string? BTeamName { get; set; }
    }
    public class NineParsonMatch : Match
    {
        public List<Player> ATeamPlayers { get; set; } = new();
        public List<Player> BTeamPlayers { get; set; } = new();
    }
}
