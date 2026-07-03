namespace CTMS.Share.Helpers
{
    /// <summary>
    /// 單一醫院定義。IsPrefixOwner = false 表示共用 prefix 的分院（例如柳營奇美沿用奇美的 CHIMEIH），
    /// 只參與院別下拉與院別→prefix 對應；流水號、隨機表、抽血檔、儀表板一律歸屬 prefix 擁有者。
    /// </summary>
    public sealed class HospitalDefinition
    {
        public string Prefix { get; init; } = "";
        public string DisplayName { get; init; } = "";
        public string ShortName { get; init; } = "";
        public string SheetEarly { get; init; } = "";
        public string SheetAdvance { get; init; } = "";
        public string BloodHematologyFile { get; init; } = "";
        public string BloodBiochemistryFile { get; init; } = "";
        public string CounterKey { get; init; } = "";
        public bool IsPrefixOwner { get; init; } = true;
    }

    /// <summary>
    /// 集中化的醫院清單：新增醫院時只需在 All 加一筆（必要時於 MagicObjectHelper 補常數與資料檔），
    /// 不需再修改散落各處的 switch / if。
    /// </summary>
    public static class HospitalRegistry
    {
        public static readonly IReadOnlyList<HospitalDefinition> All = new List<HospitalDefinition>
        {
            new()
            {
                Prefix = MagicObjectHelper.prefix成大醫院,
                DisplayName = "成大醫院",
                ShortName = MagicObjectHelper.PrefixSheetName成大醫院,
                SheetEarly = MagicObjectHelper.Sheet成大Early,
                SheetAdvance = MagicObjectHelper.Sheet成大Advance,
                BloodHematologyFile = MagicObjectHelper.成醫抽血檢驗血液File,
                BloodBiochemistryFile = MagicObjectHelper.成醫抽血檢驗生化File,
                CounterKey = "NCKUH成大",
            },
            new()
            {
                Prefix = MagicObjectHelper.prefix奇美醫院,
                DisplayName = "奇美醫院",
                ShortName = MagicObjectHelper.PrefixSheetName奇美醫院,
                SheetEarly = MagicObjectHelper.Sheet奇美Early,
                SheetAdvance = MagicObjectHelper.Sheet奇美Advance,
                BloodHematologyFile = MagicObjectHelper.奇美抽血檢驗血液File,
                BloodBiochemistryFile = MagicObjectHelper.奇美抽血檢驗生化File,
                CounterKey = "CHIMEIH奇美",
            },
            new()
            {
                Prefix = MagicObjectHelper.prefix郭綜合醫院,
                DisplayName = "郭綜合醫院",
                ShortName = MagicObjectHelper.PrefixSheetName郭綜合醫院,
                SheetEarly = MagicObjectHelper.Sheet郭綜合Early,
                SheetAdvance = MagicObjectHelper.Sheet郭綜合Advance,
                BloodHematologyFile = MagicObjectHelper.郭綜合抽血檢驗血液File,
                BloodBiochemistryFile = MagicObjectHelper.郭綜合抽血檢驗生化File,
                CounterKey = "KGH郭綜合",
            },
            new()
            {
                Prefix = MagicObjectHelper.prefix奇美醫院,
                DisplayName = "柳營奇美醫院",
                ShortName = "柳營奇美",
                IsPrefixOwner = false,
            },
            new()
            {
                Prefix = MagicObjectHelper.prefix高雄榮總醫院,
                DisplayName = "高雄榮民總醫院",
                ShortName = MagicObjectHelper.PrefixSheetName高雄榮總醫院,
                SheetEarly = MagicObjectHelper.Sheet高榮Early,
                SheetAdvance = MagicObjectHelper.Sheet高榮Advance,
                BloodHematologyFile = MagicObjectHelper.高榮抽血檢驗血液File,
                BloodBiochemistryFile = MagicObjectHelper.高榮抽血檢驗生化File,
                CounterKey = "KSVGH高榮",
            },
            new()
            {
                Prefix = MagicObjectHelper.prefix嘉義長庚醫院,
                DisplayName = "嘉義長庚紀念醫院",
                ShortName = MagicObjectHelper.PrefixSheetName嘉義長庚醫院,
                SheetEarly = MagicObjectHelper.Sheet嘉長Early,
                SheetAdvance = MagicObjectHelper.Sheet嘉長Advance,
                BloodHematologyFile = MagicObjectHelper.嘉長抽血檢驗血液File,
                BloodBiochemistryFile = MagicObjectHelper.嘉長抽血檢驗生化File,
                CounterKey = "CYCGMH嘉長",
            },
        };

        /// <summary>prefix 擁有者（排除共用 prefix 的分院）：流水號、隨機表、抽血檔、儀表板迭代用。</summary>
        public static IEnumerable<HospitalDefinition> PrefixOwners => All.Where(x => x.IsPrefixOwner);

        public static HospitalDefinition? GetByDisplayName(string 院別)
            => All.FirstOrDefault(x => x.DisplayName == 院別);

        public static HospitalDefinition? GetOwnerByPrefix(string prefix)
            => PrefixOwners.FirstOrDefault(x => x.Prefix == prefix);

        public static HospitalDefinition? GetOwnerBySubjectNo(string subjectNo)
            => PrefixOwners.FirstOrDefault(x => subjectNo.Contains(x.Prefix));

        /// <summary>
        /// 儀表板正規化：Patient.醫院 → prefix 擁有者短名（柳營奇美醫院 → 奇美）。
        /// 先做 DisplayName 精確比對（「高雄榮民總醫院」不含短名「高榮」，Contains 會漏），
        /// Contains 短名僅留作既有資料的容錯 fallback。
        /// </summary>
        public static string? NormalizeToOwnerShortName(string hospital)
        {
            var exact = All.FirstOrDefault(x => x.DisplayName == hospital);
            if (exact != null)
            {
                return GetOwnerByPrefix(exact.Prefix)?.ShortName;
            }

            return PrefixOwners.FirstOrDefault(x => hospital.Contains(x.ShortName))?.ShortName;
        }
    }
}
