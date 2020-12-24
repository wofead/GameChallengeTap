/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Fight
{
    public partial class UI_AirPlaneButton : GButton
    {
        public GGraph m_airPlaneGraph;
        public const string URL = "ui://5dhr4gxbc5im2";

        public static UI_AirPlaneButton CreateInstance()
        {
            return (UI_AirPlaneButton)UIPackage.CreateObject("Fight", "AirPlaneButton");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_airPlaneGraph = (GGraph)GetChildAt(0);
        }
    }
}