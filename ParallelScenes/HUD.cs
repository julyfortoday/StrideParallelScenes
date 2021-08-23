using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.UI.Controls;
using Stride.UI;
using Stride.Core.Diagnostics;
using Stride.Engine.Events;

namespace ParallelScenes
{
    public class HUD : SyncScript
    {
        public static EventKey NextSceneEventKey = new EventKey("Global", "NextScene");
        public static EventKey PreviousSceneEventKey = new EventKey("Global", "PreviousScene");
        private UIPage page;

        public override void Start()
        {
            page = Entity.Get<UIComponent>().Page;
            var rootElement = page.RootElement;
            var nextButton = rootElement.FindVisualChildOfType<Button>("NextButton");
            var previousButton = rootElement.FindVisualChildOfType<Button>("PreviousButton");

            var editBox = rootElement.FindVisualChildOfType<Button>("EditText");

            Log.Debug("Click");
            nextButton.Click += delegate
            {
                NextSceneEventKey.Broadcast();
            };
            previousButton.Click += delegate
            {
                PreviousSceneEventKey.Broadcast();
            };
        }

        public override void Update()
        {
        }
    }
}
