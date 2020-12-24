/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Fight
{
    public partial class UI_SkillButton : GButton
    {
        public GGraph m_skillGraph;
        public const string URL = "ui://5dhr4gxbc5im1";

        public static UI_SkillButton CreateInstance()
        {
            return (UI_SkillButton)UIPackage.CreateObject("Fight", "SkillButton");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_skillGraph = (GGraph)GetChildAt(0);
        }
    }
}