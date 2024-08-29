using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugMenu;

namespace DebugMenu.Test
{
    public class DebugMenuTest2 : DefaultDebugPage
    {
        public override string PageTitle => "デバッグメニューテスト2";

        protected override void InitPageLayouts()
        {
            //適当にボタンを複数生成
            for (int i = 0; i < 30; i++)
            {
                AddButton($"ボタン{i + 1}", () =>
                {
                });
            }
        }
    }
}