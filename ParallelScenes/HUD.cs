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
        int clickCount = 0;

        bool created = false;

        public override void Start()
        {
            if(!created)
                WireUpHUD();
            created = true;
        }

        public void WireUpHUD()
        {
            page = Entity.Get<UIComponent>().Page;
            var rootElement = page.RootElement;
            var nextButton = rootElement.FindVisualChildOfType<Button>("NextButton");
            var previousButton = rootElement.FindVisualChildOfType<Button>("PreviousButton");

            var editBox = rootElement.FindVisualChildOfType<Button>("EditText");

            var incButton = rootElement.FindVisualChildOfType<Button>("IncrementButton");
            var incButtonText = incButton.FindVisualChildOfType<TextBlock>("IncButtonText");

            if (!created)
            {
                nextButton.Click += delegate
                {
                    NextSceneEventKey.Broadcast();
                };
                previousButton.Click += delegate
                {
                    PreviousSceneEventKey.Broadcast();
                };

                incButton.Click += delegate
                {
                    clickCount++;
                    incButtonText.Text = "click: " + clickCount;
                };
            }
        }

        public override void Update()
        {
            if(Input.Keyboard.IsKeyReleased(Keys.Left))
                PreviousSceneEventKey.Broadcast();
            if (Input.Keyboard.IsKeyReleased(Keys.Right))
                NextSceneEventKey.Broadcast();
        }
    }
}
