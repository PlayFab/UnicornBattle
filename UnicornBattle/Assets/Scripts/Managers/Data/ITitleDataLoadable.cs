using System.Collections.Generic;

namespace UnicornBattle.Managers
{
    /// <summary>
    /// Indicates that this class handles TitleData
    /// </summary>
    interface ITitleDataLoadable
    {
        void LoadTitleData(Dictionary<string, string> p_titleData);

    }
}
