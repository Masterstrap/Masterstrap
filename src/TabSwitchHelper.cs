using System.Windows;
using System.Windows.Controls;

namespace Masterstrap
{
    public static class TabSwitchHelper
    {
        public static void SwitchTab(TabControl tabControl, int tabIndex)
        {
            if (tabControl != null && tabIndex >= 0 && tabIndex <= 3)
            {
                tabControl.SelectedIndex = tabIndex;
            }
        }

        public static void SwitchToHomeTab(TabControl tabControl)
        {
            SwitchTab(tabControl, 0);
        }

        public static void SwitchToEditTab(TabControl tabControl)
        {
            SwitchTab(tabControl, 1);
        }

        public static void SwitchToSettingsTab(TabControl tabControl)
        {
            SwitchTab(tabControl, 2);
        }

        public static void SwitchToFaqTab(TabControl tabControl)
        {
            SwitchTab(tabControl, 3);
        }
    }
}
