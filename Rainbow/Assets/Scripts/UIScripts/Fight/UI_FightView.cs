/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Fight
{
    public partial class UI_FightView : GComponent
    {
        public UI_SkillButton m_skill1Btn;
        public UI_SkillButton m_skill2Btn;
        public UI_SkillButton m_skill3Btn;
        public UI_AirPlaneButton m_airPlane1Btn;
        public UI_AirPlaneButton m_airPlane2Btn;
        public UI_AirPlaneButton m_airPlane3Btn;
        public const string URL = "ui://5dhr4gxbc5im0";

        public static UI_FightView CreateInstance()
        {
            return (UI_FightView)UIPackage.CreateObject("Fight", "FightView");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_skill1Btn = (UI_SkillButton)GetChildAt(0);
            m_skill2Btn = (UI_SkillButton)GetChildAt(1);
            m_skill3Btn = (UI_SkillButton)GetChildAt(2);
            m_airPlane1Btn = (UI_AirPlaneButton)GetChildAt(3);
            m_airPlane2Btn = (UI_AirPlaneButton)GetChildAt(4);
            m_airPlane3Btn = (UI_AirPlaneButton)GetChildAt(5);
        }
    }
}