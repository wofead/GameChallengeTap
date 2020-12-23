/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_LoginView : GComponent
    {
        public GGraph m_bgGraph;
        public GTextField m_loginTxt;
        public const string URL = "ui://gpt0jv85gio50";

        public static UI_LoginView CreateInstance()
        {
            return (UI_LoginView)UIPackage.CreateObject("Login", "LoginView");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_bgGraph = (GGraph)GetChildAt(0);
            m_loginTxt = (GTextField)GetChildAt(1);
        }
    }
}